using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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

            this.mybutton.Click += this.mybutton_Click;

            this.maskLayer.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
                // 隐藏遮罩层
                this.maskLayer.Visibility = Visibility.Hidden;
            };
        }

        private void mybutton_Click(object sender, RoutedEventArgs e)
        {
            this.maskLayer.Visibility = Visibility.Visible;
        }
    }
}
