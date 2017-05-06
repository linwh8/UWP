using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
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
        String currentPath = null;
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
                BitmapImage pic = new BitmapImage(new Uri(ViewModel.SelectedItem.path));
                myImage.Source = pic;
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
            
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(App.path)));
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
                        if (currentPath == null) currentPath = @"ms-appx:///C:\Users\67517\AppData\Local\Packages\1bd0fdc8-722b-45dd-84c1-ac7af1b57f95_4xx88nz50dyqw\LocalState\1.jpg";
                        ViewModel.AddTodoItem(title.Text, details.Text, dueDate.Date.DateTime, currentPath);
                        var db = App.conn;
                        try
                        {
                            using (var TodoItem = db.Prepare(App.SQL_INSERT))
                            {
                                TodoItem.Bind(1, title.Text);
                                TodoItem.Bind(2, details.Text);
                                TodoItem.Bind(3, dueDate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                                TodoItem.Bind(4, "false");
                                TodoItem.Bind(5, currentPath);
                                TodoItem.Step();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        Frame.Navigate(typeof(MainPage), ViewModel);
                        update_message_1();
                    }
                    else
                    {
                        currentPath = ViewModel.SelectedItem.path;
                        var db = App.conn;
                        try
                        {
                            using (var TodoItem = db.Prepare(App.SQL_UPDATE))
                            {
                                TodoItem.Bind(1, title.Text);
                                TodoItem.Bind(2, details.Text);
                                TodoItem.Bind(3, dueDate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                                TodoItem.Bind(4, ViewModel.SelectedItem.title);
                                TodoItem.Bind(5, ViewModel.SelectedItem.path);
                                TodoItem.Step();

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        ViewModel.UpdateTodoItem("abc", title.Text, details.Text, dueDate.Date.DateTime, currentPath);
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
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    String path = localFolder.Path;
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    String Item_id = null;
                    if (ViewModel.AllItems.Count == 0)
                    {
                        Item_id = "1";
                    }
                    else {
                        Item_id = ("Update" == createButton.Content.ToString()) ? ViewModel.SelectedItem.id : ViewModel.AllItems[ViewModel.AllItems.Count - 1].id;
                    }
                    StorageFile localFile = await localFolder.CreateFileAsync(Item_id+".jpg",CreationCollisionOption.ReplaceExisting);
                    SaveSoftwareBitmapToFile(softwareBitmap, localFile);
                    currentPath = "ms-appx:///"+localFile.Path;
                    if (ViewModel.SelectedItem != null) ViewModel.SelectedItem.path = currentPath;
                }
            }
        }
        private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                // Set additional encoding parameters, if needed
                encoder.BitmapTransform.ScaledWidth = 350;
                encoder.BitmapTransform.ScaledHeight = 180;
                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    switch (err.HResult)
                    {
                        case unchecked((int)0x88982F81): //WINCODEC_ERR_UNSUPPORTEDOPERATION
                                                         // If the encoder does not support writing a thumbnail, then try again
                                                         // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw err;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }
            }
        }

        private void MyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            if (Window.Current.Bounds.Width >= 801) {
                currentPath = ViewModel.SelectedItem.path;
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

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            String result = String.Empty;
            StringBuilder DataQuery = new StringBuilder("%%");
            DataQuery.Insert(1,SearchBox.Text);
            var db = App.conn;
            using (var statement = db.Prepare(App.SQL_SEARCH)) {
                statement.Bind(1,DataQuery.ToString());
                statement.Bind(2,DataQuery.ToString());
                statement.Bind(3,DataQuery.ToString());
                while (SQLiteResult.ROW == statement.Step()) {
                    result += statement[0].ToString() + " ";
                    result += statement[1].ToString() + " ";
                    result += statement[2].ToString() + "\n";
                }
            }
            
            if (result == String.Empty)
            {
                var box1 = new MessageDialog("Not find").ShowAsync();
            }
            else
            {
                var box2 = new MessageDialog(result).ShowAsync();
            }
        }
    }
}
