using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups; // !!!MessageDialog
using Windows.Storage.Pickers; // !!!FileOpenPicker
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Homework2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class New_page : Page
    {
        public New_page()
        {
            this.InitializeComponent();
        }

        private void CommandBar_Opened(object sender, object e) {}

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = dueDate.Date.DateTime;
            DateTime now = DateTime.Now;
            if (title.Text.Trim() == String.Empty)
            {
                var i = new MessageDialog("Title is null!").ShowAsync();
            }
            else if (details.Text.Trim() == String.Empty) {
                var i = new MessageDialog("Detail is null!").ShowAsync();
            } else if (DateTime.Compare(t, now) > 0) {
                var i = new MessageDialog("Due date is invalid!").ShowAsync();
            } else { 
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            details.Text = "";
            dueDate.Date = DateTime.Now;
        }

        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 600;
                    await bitmapImage.SetSourceAsync(fileStream);
                    myImage.Source = bitmapImage;
                }
            }
        }
    }
}
