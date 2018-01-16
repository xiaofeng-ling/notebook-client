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
using Newtonsoft.Json;

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

            var http = new Http("http://www.qq.com", "GET");

            byte[] bytes = http.Send().GetResponseBytes();

            //FileStream fs = new FileStream("baidu.html", FileMode.OpenOrCreate);

            //fs.Write(bytes, 0, bytes.Length);

            string t = http.GetResponseString();

            MessageBox.Show(GlobalVar.sess);
        }
    }
}
