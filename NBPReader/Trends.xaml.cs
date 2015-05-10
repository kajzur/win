using NBPReader.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using WinRTXamlToolkit.Imaging;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace NBPReader
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Trends : Page
    {

        private NavigationHelper navigationHelper;
        private DataRetrieverItem currentItem;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private static ProgressBar progress;
        public static DataModel DataModel = new DataModel();
        private DataRetrieverItem parameter;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public static void activateProgressBar()
        {
            progress.IsEnabled = true;
            progress.Visibility = Visibility.Visible;
            progress.UpdateLayout();

        }

        public static void deactivateProgressBar()
        {
            progress.IsEnabled = false;
            progress.Visibility = Visibility.Collapsed;
            progress.Value = 0;
            progress.UpdateLayout();

        }
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public Trends()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            fromDatePicker.MinYear = new DateTimeOffset(new DateTime(2002, 2, 1));
            progress = progressBar;
            progress.Visibility = Visibility.Collapsed;
            progress.UpdateLayout();
            this.DataContext = Trends.DataModel;
            this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;


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
            
            navigationHelper.OnNavigatedTo(e);
            parameter = e.Parameter as DataRetrieverItem;
            currentItem = parameter;
            DataRetriever dr = new DataRetriever();
            shortcut.Text = currentItem.NazwaWaluty;
            dr.ShowInitialTrend(currentItem, chart);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void fromDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            DateTime choosen = (DateTime)e.NewDate.Date;
            DateTime firstRowInAPI = new DateTime(2002, 2, 1);
            int comparationResult = DateTime.Compare(choosen, firstRowInAPI);

            if (comparationResult < 0)
            {
                MessageDialog msgbox = new MessageDialog("Wybrana data jest wcześniejsza niż dane NBP");
                msgbox.ShowAsync();  
                fromDatePicker.Date = firstRowInAPI;
            }
            else
            {
                DataRetriever.chartValues.Clear();
                DateTime from = fromDatePicker.Date.Date;
                DateTime to = toDatePicker.Date.Date;
                
                DataRetriever dr = new DataRetriever();
                progressBar.Visibility = Visibility.Visible;
                dr.PrintGraph(from, to, currentItem, chart);
            }
        }
        public async void SaveToFile()
        {
            WriteableBitmap wb = await WinRTXamlToolkit.Composition.WriteableBitmapRenderExtensions.Render(chart);
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("JPG file", new List<string>() { ".png" });
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
           /*  using (Stream x = await file.OpenStreamForWriteAsync())
            {
                x.Seek(0, SeekOrigin.Begin);
                ms.WriteTo(x);
            }
*/
            Stream stream = wb.PixelBuffer.AsStream();
            byte[] pixels = new byte[(uint)stream.Length];
            await stream.ReadAsync(pixels, 0, pixels.Length);

            using (var writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)wb.PixelWidth,
                    (uint)wb.PixelHeight,
                    96,
                    96,
                    pixels);
                await encoder.FlushAsync();

                using (var outputStream = writeStream.GetOutputStreamAt(0))
                {
                    await outputStream.FlushAsync();
                }
            }
          
            

           ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }
        private Point initialpoint;
        private void pageRoot_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            initialpoint = e.Position;
        }

        private void pageRoot_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            
        }

        private void pageRoot_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                Point currentpoint = e.Position;
                if (currentpoint.X - initialpoint.X >= 500)//500 is the threshold value, where you want to trigger the swipe right event
                {
                    this.Frame.Navigate(typeof(MainPage));
                    e.Complete();
                }
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private void toDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            DateTime choosen = (DateTime)e.NewDate.Date;
            DateTime today = DateTime.Today;
            int comparationResult = DateTime.Compare(choosen, today);

            if (comparationResult > 0)
            {
                MessageDialog msgbox = new MessageDialog("Nie możesz przewidywać przyszłości");
                msgbox.ShowAsync();
            }
            else
            {
                DataRetriever.chartValues.Clear();
                DateTime from = fromDatePicker.Date.Date;
                DateTime to = toDatePicker.Date.Date;

                DataRetriever dr = new DataRetriever();
                progressBar.Visibility = Visibility.Visible;
                dr.PrintGraph(from, to, currentItem, chart);
            }
        }
    }
}
