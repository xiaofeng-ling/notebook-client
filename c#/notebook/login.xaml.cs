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

            var http = new Http("http://192.168.0.160/app/lease/goods/getList", "GET");

            string result = http.Send().GetResponseString();

            var r = http.GetResponseJsonObject<Result>();

            if (r == null)
            {
                MessageBox.Show("网络请求失败！");
                return ;
            }

            if (r.cn != 0)
            {
                MessageBox.Show(r.message);
            }
            else
            {
                MessageBox.Show("您已成功登陆！");
            }
        }
    }

    /// <summary>
    /// 解析的
    /// </summary>
    public class Result
    {
        public string code;
        public int cn;
        public string message;
        public Object data = new { code = "", cn = 0, message = "" };
    }
}
