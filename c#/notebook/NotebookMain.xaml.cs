using System;
using System.Collections.Generic;
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
using notebook.request;
using Newtonsoft.Json;

namespace notebook
{
    /// <summary>
    /// NoteBookMain.xaml 的交互逻辑
    /// </summary>
    public partial class NoteBookMain : Window
    {
        public NoteBookMain()
        {
            InitializeComponent();

            loadNotebook();

            this.add.Click += AddClick;
        }

        private void loadNotebook()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("token", GlobalVar.token);

            string resultStr = Http.Send(Request.baseUrl + Request.notebookMainList, "GET", parameters);
            dynamic result = JsonConvert.DeserializeObject<dynamic>(resultStr);

            // todo
           // 这里需要解析数据
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            // 填写日记本名称


            this.notebookMainList.Items.Add("234324");
        }
    }
}
