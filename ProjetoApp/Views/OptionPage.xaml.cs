using ProjetoApp.Data;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjetoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionPage : Page
    {

        ObservableCollection<PhotoData> _photos = new ObservableCollection<PhotoData>();

        Option _option;
        Flyout _flyout;

        public OptionPage()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo();

            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;

            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            var id = e.Parameter;
            _option = await DataProvider.Instance.GetOption(id as string);

            OptionNameText.Text = _option.Name.ToUpper();
            if (_option.User != null)
            {
                UserImage.ImageSource = new BitmapImage(new Uri(_option.User.Image));
            }

            for (var i = _option.Photos.Count - 1; i > -1; --i)
            {
                _photos.Add(_option.Photos[i]);
            }

            if (_photos.Count > 0)
            {
                var randomPhoto = _photos[new Random().Next(0, _photos.Count)];
                BackgroundImage.ImageSource = await GetBitmapImage(randomPhoto.Uri, randomPhoto.IsLocal);
            }
            else
            {
                // show someting
            }

            
            base.OnNavigatedTo(e);
        }

        private async Task<BitmapImage> GetBitmapImage(string path, bool isLocal)
        {
            try
            {
                if (isLocal)
                {
                    var bi = new BitmapImage();
                    var file = await StorageFile.GetFileFromPathAsync(path);
                    await bi.SetSourceAsync(await file.OpenReadAsync());
                    return bi;
                }
                else
                {
                    return new BitmapImage(new Uri(path));
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var photo = e.ClickedItem as PhotoData;

            object focusItem = null;

            await PhotoPreviewView.ShowAndWaitAsync(photo);

            // quick way to refresh binding
            var index = _photos.IndexOf(photo);
            _photos.RemoveAt(index);
            _photos.Insert(index, photo);


            await DataProvider.Instance.SavePhotoAsync(photo);
        }


        private void ImageGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var range = 5 * (180 + 16);
            if (e.NewSize.Width > range)
            {
                DescriptionText.Width = range - 16;
            }
            else
            {
                DescriptionText.Width = (int)(e.NewSize.Width / 196) * 196 - 16;
            }
        }

        private void Slideshow_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SlideshowPage), _option.Id.ToString());
        }


        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _flyout.Hide();
            var success = true;
        }


        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            DataProvider.Instance.ClearSavedFaces();
        }
    }
}