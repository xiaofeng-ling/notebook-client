using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace notebook
{
    /// <summary>
    /// login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = this.username.Text;
            string password = this.password.Text;

            var http = new Http("http://www.baidu.com", "GET");

            var result = http.Send();
           
            FileStream fs = new FileStream("baidu.html", FileMode.OpenOrCreate);

            byte[] bytes = http.GetResponseBytes();

            fs.Write(bytes, 0, bytes.Length);

            MessageBox.Show(GlobalVar.sess);
        }
    }
}
