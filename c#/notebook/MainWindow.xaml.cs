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
        public MainWindow()
        {
            InitializeComponent();

            Config config = Config.LoadConfig();
            GlobalVar.token = config.token;

            //if ("" == GlobalVar.token)
                new Login().ShowDialog();

            new NoteBookMain().ShowDialog();

            this.add.Click += this.AddClick;
            this.save.Click += this.SaveClickAsync;

            this.text.Text = GlobalVar.token;
        }

        private void SaveClickAsync(object sender, RoutedEventArgs e)
        {
            new NoteBookMain().ShowDialog();
            dynamic end = this.list.Items.Count - 1;
            this.list.Items.RemoveAt(end);
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            //this.list.Items.Add();
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

        // 登录过后的用户id
        public static int user_id = 0;
    }
}
