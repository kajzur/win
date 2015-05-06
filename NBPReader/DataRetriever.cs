
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace NBPReader
{
    class DataRetriever
    {
        private string current = @"http://www.nbp.pl/kursy/xml/LastA.xml";
        private string prefix = @"http://www.nbp.pl/kursy/xml/";
        private string datesUrl = @"http://www.nbp.pl/kursy/xml/dir.txt";
        private delegate void Callback(string result);
        private delegate void XmlCallback(XDocument x);
        public static List<ChartValue> chartValues = new List<ChartValue>();
        public DataRetriever()
        {

        }

        private async Task Get(string url, Callback callback)
        {
            HttpClient client = new HttpClient();
            string output;

            HttpResponseMessage response = await client.GetAsync(new System.Uri(url));
            response.EnsureSuccessStatusCode();
            output = await response.Content.ReadAsStringAsync();
            callback(output);
        }

        private async Task GetXml(string url, XmlCallback callback)
        {
            XDocument loadedData = XDocument.Load(url);
            callback(loadedData);
        }

        public CurrentDataSet ParseInitialXml()
        {
            XDocument loadedData = loadData(current);

            string date = (string)loadedData.Root.Element("data_publikacji");
            var data = from query in loadedData.Descendants("pozycja")
                       select new DataRetrieverItem
                       {
                           NazwaWaluty = (string)query.Element("nazwa_waluty"),
                           Przelicznik = (string)query.Element("przelicznik"),
                           KodWaluty = (string)query.Element("kod_waluty"),
                           KursSredni = (string)query.Element("kurs_sredni")
                       };
            CurrentDataSet CurrentSet = new CurrentDataSet();
            CurrentSet.items = data.ToList();
            CurrentSet.date = DateTime.Parse(date);
            return CurrentSet;
        }

        private XDocument loadData(string url)
        {
            XDocument loadedData = XDocument.Load(url);
            return loadedData;
        }

        public void SetDates(ListView lv)
        {
            Callback c = delegate(string result)
            {
                String[] all = result.Split('\n');
                string year, month, day;
                List<DateTimeWrapper> dates = new List<DateTimeWrapper>();
                foreach (string singleFile in all)
                {
                    if (!singleFile.StartsWith("a"))
                    {
                        continue;
                    }
                    String date = singleFile.Substring(5);
                    year = "20" + date.Substring(0, 2);
                    month = date.Substring(2, 2);
                    day = date.Substring(4, 2);
                    DateTime dateTimeObj = DateTime.Parse(year + "-" + month + "-" + day);
                    dates.Add(new DateTimeWrapper(dateTimeObj, singleFile));
                }
                dates.Reverse();
                lv.ItemsSource = dates;
            };
            Get(datesUrl, c);

        }

        public void RefreshMainList(string filename, ListView lv)
        {

            filename = filename.Trim(new Char[] { '\r' });
            string url = prefix + filename + ".xml";
            XmlCallback c = delegate(XDocument loadedData)
            {
                string date = (string)loadedData.Root.Element("data_publikacji");
                var data = from query in loadedData.Descendants("pozycja")
                           select new DataRetrieverItem
                           {
                               NazwaWaluty = (string)query.Element("nazwa_waluty"),
                               Przelicznik = (string)query.Element("przelicznik"),
                               KodWaluty = (string)query.Element("kod_waluty"),
                               KursSredni = (string)query.Element("kurs_sredni")
                           };
                CurrentDataSet CurrentSet = new CurrentDataSet();
                CurrentSet.items = data.ToList();
                CurrentSet.date = DateTime.Parse(date);
                lv.ItemsSource = CurrentSet.items;
            };
            GetXml(url, c);
        }

        public void PrintGraph(DateTime from, DateTime to, DataRetrieverItem currentItem, Chart chart)
        {
            Trends.activateProgressBar();
            Callback c = delegate(string result)
            {
                List<string> validNames = new List<string>();
                String[] all = result.Split('\n');
                string year, month, day;
                List<DateTimeWrapper> dates = new List<DateTimeWrapper>();
                foreach (string singleFile in all)
                {
                    if (!singleFile.StartsWith("a"))
                    {
                        continue;
                    }
                    String date = singleFile.Substring(5);
                    year = "20" + date.Substring(0, 2);
                    month = date.Substring(2, 2);
                    day = date.Substring(4, 2).Trim(new Char[] { '\r' });
                    DateTime dateTimeObj = DateTime.Parse(year + "-" + month + "-" + day);
                    if (DateTime.Compare(dateTimeObj, from) >= 0 && DateTime.Compare(dateTimeObj, to) <= 0)
                    {
                        validNames.Add(singleFile.Trim(new Char[] { '\r' }));
                    }
                }
                this.RetrieveValuesForChart(validNames, currentItem, from, to, chart);

            };

            Get(datesUrl, c);
        }


        private async void RetrieveValuesForChart(List<string> filenames, DataRetrieverItem currentItem, DateTime from, DateTime to, Chart chart)
        {

            string output;
            CacheController cc = new CacheController(currentItem.KodWaluty, from, to);
            if (await cc.IsCacheAvailable())
            {
                DataRetriever.chartValues = await cc.GetCachedValues();
            }
            else
            {

                foreach (string filename in filenames)
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(new System.Uri(prefix + filename + ".xml"));
                    response.EnsureSuccessStatusCode();
                    output = await response.Content.ReadAsStringAsync();

                    XDocument loadedData = XDocument.Parse(output);
                    string date = (string)loadedData.Root.Element("data_publikacji");
                    var data = from query in loadedData.Descendants("pozycja")
                               select new ChartValue
                               {
                                   KodWaluty = (string)query.Element("kod_waluty"),
                                   Value = (string)query.Element("kurs_sredni"),
                                   Date = date
                               };
                    List<ChartValue> values = data.ToList();
                    values = values.Where(x => x.KodWaluty.Equals(currentItem.KodWaluty)).ToList<ChartValue>();
                    ChartValue retreievedValue = values.ElementAt(0);
                    retreievedValue.Date = date;
                    DataRetriever.chartValues.Add(retreievedValue);
                }

                await cc.CreateCache(DataRetriever.chartValues);
            }
            Trends.deactivateProgressBar();
            (chart.Series[0] as LineSeries).ItemsSource = DataRetriever.chartValues;
            (chart.Series[0] as LineSeries).Refresh();
            (chart.Series[0] as LineSeries).UpdateLayout();
        }

        internal void ShowInitialTrend(DataRetrieverItem currentItem, Chart chart)
        {
            ChartValue cv = new ChartValue("", currentItem.KursSredni);
            DataRetriever.chartValues.Clear();
            DataRetriever.chartValues.Add(cv);
            (chart.Series[0] as LineSeries).ItemsSource = DataRetriever.chartValues;
            (chart.Series[0] as LineSeries).Refresh();
            (chart.Series[0] as LineSeries).UpdateLayout();
        }
    }
}
