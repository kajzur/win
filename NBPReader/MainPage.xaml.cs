using NBPReader.Common;
using System;
using System.IO;
using System.Xml.Serialization;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace NBPReader
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        Windows.Storage.ApplicationDataContainer roamingSettings =
                Windows.Storage.ApplicationData.Current.RoamingSettings;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public static void CheckInternetConnection()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            if (!internet)
            {
                MessageDialog msgbox = new MessageDialog("Aplikacja wymaga połączenie z internetem");
                msgbox.Commands.Add(new UICommand("Exit", new UICommandInvokedHandler(msgBox_Handler)));
                msgbox.DefaultCommandIndex = 1;
                msgbox.ShowAsync();
                throw new Exception("Internet Connection required.");
            }
        }

        private static void msgBox_Handler(IUICommand command)
        {
            String label = command.Label;
            if (label.Equals("Retry"))
            {
                CheckInternetConnection();
            }
            else if (label.Equals("Exit"))
            {
                App.Current.Exit();
            }
        }
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {

    
                this.InitializeComponent(); 
                this.navigationHelper = new NavigationHelper(this);
                this.navigationHelper.LoadState += navigationHelper_LoadState;
                this.navigationHelper.SaveState += navigationHelper_SaveState;  
                DataRetriever dr = new DataRetriever();
                CurrentDataSet cds = dr.ParseInitialXml();
                if (cds != null)
                {
                    currentList.ItemsSource = cds.items;
                    dr.SetDates(dates);

                    this.DataContext = cds;
                }
     
   

        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }


        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Trends.DataModel.ChartValues.Clear();
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void dates_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var rawFileName = (e.AddedItems[0] as DateTimeWrapper).RawFileName;
                roamingSettings.Values["selectedDate"] = dates.SelectedIndex;
                DataRetriever dr = new DataRetriever();
                dr.RefreshMainList(rawFileName, currentList);
            }  
        }

        private void currentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                DataRetrieverItem item = (e.AddedItems[0] as DataRetrieverItem);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataRetrieverItem));
                StringWriter textWriter = new StringWriter();
                xmlSerializer.Serialize(textWriter, item);
                if (this.Frame != null)
                {
                    this.Frame.Navigate(typeof(Trends), textWriter.ToString());
                }
            }  
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }
    }
}
