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
using notebook.request;

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
            string password = this.password.Password;

            try
            {
                IDictionary<string, string> data = new Dictionary<string, string>();
                data.Add("email", username);
                data.Add("password", password);

                string temp = Http.Send("http://notebook.test/api/login", "POST", data);
                LoginResult result = JsonConvert.DeserializeObject<LoginResult>(temp);

                Config config = Config.LoadConfig();

                if (result.code == 1000)
                {
                    GlobalVar.token = result.data.ToString();
                    config.token = GlobalVar.token;
                    config.saveConfig();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.message);
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
