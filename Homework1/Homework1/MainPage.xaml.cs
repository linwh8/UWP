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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Homework1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private delegate string AnimalSaying(object sender, myEventArgs e);
        private event AnimalSaying Say;
        private int times = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        interface Animal {
            string saying(object sender, myEventArgs e);
        }

        class pig : Animal {
            TextBlock word;
            public pig(TextBlock w) {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e) {
                this.word.Text += "第" + e.t + "次" + "Pig: I am a pig\n";
                return "";
            }
        }

        class dog : Animal {
            TextBlock word;
            public dog(TextBlock w) {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e) {
                this.word.Text += "第" + e.t + "次" + "Dog: I am a dog\n";
                return "";
            }
        }

        class cat : Animal {
            TextBlock word;
            public cat(TextBlock w) {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e) {
                this.word.Text += "第" + e.t + "次" + "Cat: I am a cat\n";
                return "";
            }
        }
        private cat c;
        private dog d;
        private pig p;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (times % 18 == 0)
            {
                textBlock.Text = "";
            }
            Random ran = new Random();
            int RandomKey = ran.Next(0, 6);
            if (RandomKey == 0 || RandomKey == 3)
            {
                c = new cat(textBlock);
                Say += new AnimalSaying(c.saying);
            }
            else if (RandomKey == 1 || RandomKey == 4)
            {
                d = new dog(textBlock);
                Say += new AnimalSaying(d.saying);
            }
            else {
                p = new pig(textBlock);
                Say += new AnimalSaying(p.saying);
            }
            Say(this, new myEventArgs(times++));
            Say = null; ;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int flag = 0;
            if (times % 18 == 0) {
                textBlock.Text = "";
            }
            if (textBox.Text == "cat")
            {
                c = new cat(textBlock);
                Say += new AnimalSaying(c.saying);
            }
            else if (textBox.Text == "dog")
            {
                d = new dog(textBlock);
                Say += new AnimalSaying(d.saying);
            }
            else if (textBox.Text == "pig")
            {
                p = new pig(textBlock);
                Say += new AnimalSaying(p.saying);
            }
            else {
                flag = 1;
            }
            if (flag != 1)
            {
                Say(this, new myEventArgs(times++));
                Say = null;
            }
            textBox.Text = "";
        }

        class myEventArgs: EventArgs {
            public int t;
            public myEventArgs(int tt) {
                this.t = tt;
            }
        }
    }
}
