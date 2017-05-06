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
            if (cb.IsChecked == true)
            {
                myline1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //App.SomeImportantValue = true;
            }
            else
            {
                myline1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //App.SomeImportantValue = false;
            }
        }
    }
}
