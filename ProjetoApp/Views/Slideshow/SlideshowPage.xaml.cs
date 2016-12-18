using ProjetoApp.Data;
using Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjetoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SlideshowPage : Page
    {
        Option _option;
        DispatcherTimer _timer;

        bool _navVisible;

        public SlideshowPage()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo();

            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;
            this.SizeChanged += (s, e) => UpdateSize();
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonInactiveForegroundColor = Colors.LightGray;

                    titleBar.BackgroundColor = Colors.Transparent;
                    titleBar.InactiveBackgroundColor = Colors.Transparent;
                }
            }
            
            var id = e.Parameter;
            _option = await DataProvider.Instance.GetOption(id as string);

            PhotoTimeline.ItemsSource = _option.Photos;

            UpdateSize();
            

            SlideTextBlock.Text = $"Slide {PhotoTimeline.CurrentItemIndex + 1} of {_option.Photos.Count}";
            PhotoTimeline.ItemIndexChanged += TimelinePanel_ItemIndexChanged;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            PointerMoved += SlideshowPage_PointerMoved;
            KeyDown += SlideshowPage_KeyDown;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = false;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {

                    var brandColor = (App.Current.Resources["BrandColor"] as SolidColorBrush).Color;
                    var brandColorLight = (App.Current.Resources["BrandColorLight"] as SolidColorBrush).Color;

                    titleBar.ButtonBackgroundColor = brandColor;
                    titleBar.ButtonInactiveBackgroundColor = brandColorLight;

                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonInactiveForegroundColor = Colors.Black;

                    titleBar.BackgroundColor = brandColor;
                    titleBar.InactiveBackgroundColor = brandColorLight;
                }
            }

            base.OnNavigatedFrom(e);
        }

        private void UpdateSize()
        {
            var size = Math.Min(ActualHeight, ActualWidth) - 20;

            foreach (var view in PhotoTimeline.FindChildren<SlideshowSlideView>())
            {
                view.Height = view.Width = size;
            }

            if (PhotoTimeline.TimelinePanel != null) PhotoTimeline.TimelinePanel.ItemSpacing = ActualWidth / 2;
        }

        private void TimelinePanel_ItemIndexChanged(object sender, TimelinePanelItemIndexEventArgs e)
        {
            SlideTextBlock.Text = $"Slide {e.NewValue + 1} of {_option.Photos.Count}";
        }

        private void SlideshowPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.GamepadRightShoulder)
            {
                Next();
                e.Handled = true;
            }
            else if (e.Key == Windows.System.VirtualKey.GamepadLeftShoulder)
            {
                Previous();
                e.Handled = true;
            }
            else if (!_navVisible)
            {
                _navVisible = true;
                e.Handled = true;
               
                ControlsContainer.IsHitTestVisible = true;
                ControlsContainer.Fade(1, 200).Start();
            }
            
            _timer.Start();
        }

        private void SlideshowPage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_navVisible)
            {
                _navVisible = true;
                ControlsContainer.IsHitTestVisible = true;
                ControlsContainer.Fade(1, 200).Start();
            }

            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            _timer.Stop();
            ControlsContainer.Fade(0, 1000).Start();
            ControlsContainer.IsHitTestVisible = false;
            _navVisible = false;
        }

        private void SlideshowSlideView_Loaded(object sender, RoutedEventArgs e)
        {
            var size = Math.Min(ActualHeight, ActualWidth) - 20;
            var view = sender as SlideshowSlideView;
            view.Height = view.Width = size;
        }

        private void Next()
        {
            if (PhotoTimeline.CurrentItemIndex != _option.Photos.Count)
            {
                PhotoTimeline.CurrentItemIndex++;
                SendIndexUpdate();
            }
        }

        private void Previous()
        {
            if (PhotoTimeline.CurrentItemIndex > 0)
            {
                PhotoTimeline.CurrentItemIndex--;
                SendIndexUpdate();
            }
        }

        private async Task<bool> SendIndexUpdate()
        {
            var message = new ValueSet();
            message.Add("index", PhotoTimeline.CurrentItemIndex);
            var success = true;

            return success;
        }

        
        private async Task HandleStrokeData(byte[] data, int index)
        {
            if (index == PhotoTimeline.CurrentItemIndex)
            {
                var view = PhotoTimeline.FindChildren<SlideshowSlideView>().ElementAt(index);
                await view.UpdateStrokes(data);
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

    }
}
