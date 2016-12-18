using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;
using ProjetoApp.Data;
using Windows.Devices.Geolocation;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Controls.Maps;
using System.Diagnostics;
using Windows.Services.Maps;

namespace ProjetoApp
{
    public sealed partial class MainPage : Page
    {
        Option CurrentOption;

        private string _title;

        public static MainPage Instance;

        public MainPage()
        {
            this.InitializeComponent();
            Map.MapServiceToken = Keys.MapServiceToken;

            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction"))
                Map.AllowFocusOnInteraction = true;
            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "IsFocusEngagementEnabled"))
                Map.IsFocusEngagementEnabled = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Instance = this;
            // Set the map location.
            /* var locationTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Map.LandmarksVisible = true;
                LoadMapElements();
                if (e.Parameter != null && e.Parameter is string)
                {
                    var par = (e.Parameter as string).Split(':');
                    if (par[0] == "showmap")
                    {
                        MoveMap(par[1], par[2]);
                    }
                }

                var currentLocation = await Maps.GetCurrentLocationAsync();

                if (currentLocation != null)
                {
                    var icon = new MapIcon();
                    icon.Location = currentLocation;
                    icon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                    icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Square44x44Logo.targetsize-30.png"));

                    Map.MapElements.Add(icon);
                }
            });*/

            CurrentOption = await DataProvider.Instance.GetCurrentOption();
            List<UIElement> elementsToShow = new List<UIElement>();

            if (await DataProvider.Instance.GetCurrentOption() == null)
            {
                elementsToShow.Add(MainControlsNewOptionButtonContainer);
                elementsToShow.Add(MainControlsViewOldOptionsButtonContainer);
            }
            else
            {
                elementsToShow.Add(MainControlsViewOldOptionsButtonContainer);
                elementsToShow.Add(MainControlsCaptureButtonContainer);
                elementsToShow.Add(MainControlsUploadButtonContainer);
                elementsToShow.Add(MainControlsBrowseButtonContainer);
            }

            for (var i = 0; i < elementsToShow.Count; i++)
            {
                var elem = elementsToShow[i];
                elem.Opacity = 0;
                elem.Visibility = Visibility.Visible;
                elem.Offset(0, 20, 0).Then().Offset().Fade(1).SetDuration(200).SetDelay(i * 100).Start();
            }
            
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Instance = null;
            base.OnNavigatingFrom(e);
        }

        private async void LoadMapElements()
        {
            var Options = await DataProvider.Instance.GetFriendsOptions();

            foreach (var Option in Options.OrderBy(a => a.Location.Longitude))
            {
                var point = new Geopoint(Option.Location);
                var ellipse = new Ellipse()
                {
                    Height = 50,
                    Width = 50,
                    Stroke = App.Current.Resources["BrandColor"] as SolidColorBrush,
                    StrokeThickness = 2,
                    Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(Option.User.Image))
                    },
                };

                var button = new Button();
                button.DataContext = Option;
                button.Template = App.Current.Resources["MapButtonStyle"] as ControlTemplate;
                button.UseSystemFocusVisuals = false;
                button.Content = ellipse;
                button.Click += MapButtonClicked;

                Map.Children.Add(button);
                MapControl.SetLocation(button, point);
                MapControl.SetNormalizedAnchorPoint(button, new Point(0.5, 0.5));
            }

            //var box = new GeoboundingBox(
            //    new BasicGeoposition()
            //    {
            //        Latitude = Options.OrderBy(a => a.Location.Latitude).Select(a => a.Location.Latitude).Last(),
            //        Longitude = Options.OrderBy(a => a.Location.Longitude).Select(a => a.Location.Longitude).First()
            //    },
            //    new BasicGeoposition()
            //    {
            //        Latitude = Options.OrderBy(a => a.Location.Latitude).Select(a => a.Location.Latitude).First(),
            //        Longitude = Options.OrderBy(a => a.Location.Longitude).Select(a => a.Location.Longitude).Last()
            //    });

            //await Map.TrySetViewBoundsAsync(box, new Thickness(40), MapAnimationKind.Default);
        }

        private void Map_ActualCameraChanged(MapControl sender, MapActualCameraChangedEventArgs args)
        {
            var buttons = new Dictionary<Point, Button>();

            foreach (var child in Map.Children)
            {

                var btn = child as Button;
                Point point;
                try
                {
                    point = btn.TransformToVisual(Map).TransformPoint(new Point());
                }
                catch (Exception)
                {
                    return;
                }

                if (point.X > 0 && point.X <= Map.ActualWidth &&
                    point.Y > 0 && point.Y <= Map.ActualHeight)
                {
                    btn.IsTabStop = true;
                    buttons.Add(point, btn);
                }
                else
                {
                    btn.IsTabStop = false;
                }
            }

            Button previosBtn = null;
            var orderedButtons = buttons.OrderBy(b => b.Key.X);

            foreach (var point in orderedButtons)
            {
                var button = point.Value;

                button.XYFocusUp = button;
                button.XYFocusRight = button;
                button.XYFocusLeft = previosBtn != null ? previosBtn : button;
                button.XYFocusDown = MainControlsViewOldOptionsButton;

                if (previosBtn != null)
                {
                    previosBtn.XYFocusRight = button;
                }

                previosBtn = button;
            }

            if (orderedButtons.Count() > 1)
            {
                orderedButtons.Last().Value.XYFocusRight = orderedButtons.First().Value;
                orderedButtons.First().Value.XYFocusLeft = orderedButtons.Last().Value;
            }
        }

        public async Task<bool> MoveMap(string location, string type = "")
        {
            var mapLocations = await MapLocationFinder.FindLocationsAsync(location, null);

            double zoomLevel;
            switch (type)
            {
                case "city":
                    zoomLevel = 10;
                    break;
                case "continent":
                    zoomLevel = 4;
                    break;
                case "country":
                    zoomLevel = 6;
                    break;
                case "us_state":
                    zoomLevel = 7;
                    break;
                default:
                    zoomLevel = 4;
                    break;
            }

            return await Map.TrySetViewAsync(mapLocations.Locations.First().Point, zoomLevel).AsTask();
        }

        public Task<bool> MoveMap(Option Option)
        {
            return Map.TrySetViewAsync(new Geopoint(Option.Location), 12).AsTask();
        }

        private void MapButtonClicked(object sender, RoutedEventArgs e)
        {
            var Option = (sender as Button).DataContext as Option;

            Frame.Navigate(typeof(OptionPage), Option.Id.ToString());
        }

        private void CaptureButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CapturePage));
        }

        private async void UploadButtonClick(object sender, RoutedEventArgs e)
        {

            (sender as Button).IsEnabled = false;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();

            var photos = new List<PhotoData>();

            if (files != null && files.Count > 0)
            {
                var folder = await ObjectStorageHelper.GetDataSaveFolder();

                foreach (var file in files)
                {
                    var newFile = file;

                    if ((await file.GetParentAsync()).Path != folder.Path)
                        newFile = await file.CopyAsync(folder);

                    var photo = new PhotoData()
                    {
                        DateTime = DateTime.Now,
                        Uri = newFile.Path,
                    };

                    //photo.ThumbnailUri = await VisionAPI.Instance.GetThumbnail(await newFile.OpenReadAsync(), newFile.DisplayName + "_thumb.jpg");

                    photos.Add(photo);
                }

                //await Data.Instance.SavePhotosAsync(photos);

                //Frame.Navigate(typeof(OptionPage), null);
            }
            else
            {
                (sender as Button).IsEnabled = true;
            }
        }

        private void PhotosButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OptionPage), CurrentOption.Id.ToString());
        }

        private void MainControlsViewOldOptionsButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {

            // hide MapToolbar controls
            var toolbar = Map.FindDescendant<StackPanel>();
            if (toolbar != null)
            {
                toolbar.Visibility = Visibility.Collapsed;
            }

            var swapchainpanel = Map.FindDescendant<SwapChainPanel>();
            if (swapchainpanel != null && swapchainpanel.Parent != null)
            {
                var grid = swapchainpanel.Parent as Grid;

                var border = new Border()
                {
                    IsHitTestVisible = false,
                    Background = App.Current.Resources["BrandColor"] as SolidColorBrush,
                    Opacity = 0.4
                };

                grid.Children.Add(border);
            }
        }


        private void MainControlsNewOptionButton_Click(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.Fade(0).Scale(0.5f, 0.5f, (float)MainScrollViewer.ActualWidth / 2, (float)MainScrollViewer.ActualHeight / 2).SetDuration(200).Start();
            MainScrollViewer.IsHitTestVisible = false;

            _title = MainControlsTitle.Text;
            MainControlsTitle.Text = "Nova Opção";

            FindName("CreateOptionSection");
            CreateOptionSection.Scale(1.2f, 1.2f, (float)ActualWidth / 2, (float)CreateOptionSection.ActualHeight / 2, 0)
                .Then().Fade(1).Scale(1f, 1f, (float)ActualWidth / 2, (float)CreateOptionSection.ActualHeight / 2, 0)
                .SetDuration(300).Start();
            CreateOptionSection.IsHitTestVisible = true;

            (App.Current as App).BackRequested += HideCreateNewOptionSection;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            CreateOptionTextBox.Focus(FocusState.Keyboard);
        }

        private void HideCreateNewOptionSection(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            (App.Current as App).BackRequested -= HideCreateNewOptionSection;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

            MainControlsTitle.Text = _title;

            MainScrollViewer.Fade(1).Scale(1f, 1f, (float)MainScrollViewer.ActualWidth / 2, (float)MainScrollViewer.ActualHeight / 2).SetDuration(300).Start();
            MainScrollViewer.IsHitTestVisible = true;

            CreateOptionSection.Scale(1.2f, 1.2f, (float)CreateOptionSection.ActualWidth / 2, (float)CreateOptionSection.ActualHeight / 2)
                .Fade(0).SetDuration(200).Start();
            CreateOptionSection.IsHitTestVisible = false;
        }

        private async void CreateNewOptionButton_Click(object sender, RoutedEventArgs e)
        {
            CreateOptionTextBox.IsEnabled = false;
            CreateNewOptionButton.IsEnabled = false;

            if (!string.IsNullOrWhiteSpace(CreateOptionTextBox.Text))
            {
                var Option = await DataProvider.Instance.CreateNewOption(CreateOptionTextBox.Text);
                Frame.Navigate(typeof(OptionPage), Option.Id.ToString());
            }

            CreateOptionTextBox.IsEnabled = true;
            CreateNewOptionButton.IsEnabled = true;
        }
    }
}
