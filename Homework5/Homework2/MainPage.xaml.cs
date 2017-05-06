using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Homework2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel =  new ViewModels.TodoItemViewModel() ;
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(New_page), ViewModel);
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
            if (ViewModel.SelectedItem == null)
            {
                createButton.Content = "Create";
                //var i = new MessageDialog("Welcome!").ShowAsync();
            }
            else
            {
                createButton.Content = "Update";
                title.Text = ViewModel.SelectedItem.title;
                details.Text = ViewModel.SelectedItem.description;
                dueDate.Date = ViewModel.SelectedItem.date;
            }
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            DataRequest request = args.Request;
            request.Data.Properties.Title = App.title;
            request.Data.Properties.Description = App.details;
            DataRequestDeferral deferal = request.GetDeferral();
            /* List<IStorageItem> imageItems = new List<IStorageItem> { App._tempExportFile };
             request.Data.SetStorageItems(imageItems);

             RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(App._tempExportFile);
             request.Data.Properties.Thumbnail = imageStreamRef;
             request.Data.SetBitmap(imageStreamRef);
             */
           request.Data.SetText(App.details);
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/background1.jpg")));
            deferal.Complete();
        }

        private void CommandBar_Opened(object sender, object e) { }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = dueDate.Date.DateTime;
            DateTime now = DateTime.Now;
            if (title.Text.Trim() == String.Empty)
            {
                var i = new MessageDialog("Title is null!").ShowAsync();
            }
            else if (details.Text.Trim() == String.Empty)
            {
                var i = new MessageDialog("Detail is null!").ShowAsync();
            }
            else if (DateTime.Compare(t, now) > 0)
            {
                var i = new MessageDialog("Due date is invalid!").ShowAsync();
            }
            else
            {
                var str = (String)createButton.Content;
                if (ViewModel != null)
                {
                    if (str == "Create")
                    {
                        ViewModel.AddTodoItem(title.Text, details.Text);
                        Frame.Navigate(typeof(MainPage), ViewModel);
                        update_message_1();
                    }
                    else
                    {
                        ViewModel.UpdateTodoItem("abc", title.Text, details.Text, dueDate.Date.DateTime);
                        Frame.Navigate(typeof(MainPage), ViewModel);
                        update_message_1();
                    }
                }
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

        private void MyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            if (Window.Current.Bounds.Width >= 801) {
                Frame.Navigate(typeof(MainPage), ViewModel);
            } else {
                Frame.Navigate(typeof(New_page), ViewModel);
            }
        }
        
        private void update_message_1()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(System.IO.File.ReadAllText("Tile.xml"));
            XmlNodeList textElements = document.GetElementsByTagName("text");
            var count = (int)ViewModel.AllItems.Count;
            if (count == 0) return;
            textElements[2].InnerText = ViewModel.AllItems[count - 1].title;
            textElements[3].InnerText = ViewModel.AllItems[count - 1].description;
            textElements[4].InnerText = ViewModel.AllItems[count - 1].title;
            textElements[5].InnerText = ViewModel.AllItems[count - 1].description;
            textElements[6].InnerText = ViewModel.AllItems[count - 1].title;
            textElements[7].InnerText = ViewModel.AllItems[count - 1].description;
            var tileNotification = new TileNotification(document);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }
    }
}
