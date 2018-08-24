using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace notebook
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int temp = 1;

        public MainWindow()
        {
            InitializeComponent();

            this.add.Click += this.AddClick;
            this.save.Click += this.SaveClickAsync;
        }

        private async void SaveClickAsync(object sender, RoutedEventArgs e)
        {

            dynamic end = this.list.Items.Count - 1;
            this.list.Items.RemoveAt(end);
            MessageBox.Show("123");

        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.list.Items.Add(this.temp++);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
        }
    }

    // 全局变量类
    public class GlobalVar
    {
        // 登录过后的token
        public static string token = "";
    }
}
