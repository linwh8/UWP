using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Edi.UWP.Helpers;
using Windows.Storage;
using Windows.UI.Popups;
using Homework2.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Homework2
{
    public sealed partial class MyUserControl : UserControl
    {
        public Models.TodoItem TodoItem { get { return this.DataContext as Models.TodoItem; } }
        public MyUserControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            //MyCheckBox.IsChecked = App.SomeImportantValue;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            var s = sender as FrameworkElement;
            var item = (Models.TodoItem)s.DataContext;
            //var i = new MessageDialog(item.title).ShowAsync();
            if (cb.IsChecked == true)
            {
               // myline1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                item.completed = true;
                //App.SomeImportantValue = true;
                var db = App.conn;
                try
                {
                    using (var TodoItem = db.Prepare(App.SQL_UPDATE_COMPLETE))
                    {
                        TodoItem.Bind(1, "true");
                        TodoItem.Bind(2, item.title);
                        TodoItem.Step();

                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                //myline1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                item.completed = false;
                //App.SomeImportantValue = false;
                var db = App.conn;
                try
                {
                    using (var TodoItem = db.Prepare(App.SQL_UPDATE_COMPLETE))
                    {
                        TodoItem.Bind(1, "false");
                        TodoItem.Bind(2, item.title);
                        TodoItem.Step();

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            var item = (Models.TodoItem)s.DataContext;
            App.title = item.title;
            App.details = item.description;
            App.path = item.path;
            DataTransferManager.ShowShareUI();
        }

    }
}
