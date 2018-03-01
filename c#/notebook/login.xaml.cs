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
using System.Reflection;
using System.Dynamic;
using Newtonsoft.Json.Linq;

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

        private void Login_Click(object sender, RoutedEventArgs eventArgs)
        {
            string username = this.username.Text;
            string password = this.password.Text;

            var http = new Http("http://192.168.0.160/panel/lease/goods/getList", "POST");

            IDictionary<string, string> par = new Dictionary<string, string>
            {
                { "panel_sess", "f5b8tkCdCCFYrT0jxPX%2FVzgL1s%2BUhg1lvfjx3OwDBiFfROrC3EJD4DFqhZagDabw1b5ljAKLonyLekO5" }
            };

            try
            {

                dynamic result = http.Send(par).GetResponseJsonObject<dynamic>();

                if (result == null)
                {
                    MessageBox.Show("网络请求失败！");
                    return;
                }

                if (result.cn != 0)
                {
                    MessageBox.Show(result.message);
                }
                else
                {
                    MessageBox.Show(result.data.list[0].name.Value as string);
                    MessageBox.Show("您已成功登陆！");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }

    /// <summary>
    /// 解析的结果类
    /// </summary>
    public class Result
    {
        public string code;
        public int cn;
        public string message;
        public Object data = new { pageIndex = 0, pageSize = 0, total = 0, list = ""};
    }
}
