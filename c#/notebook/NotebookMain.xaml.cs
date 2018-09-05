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

            LoadNotebook();

            this.add.Click += AddClick;
            this.del.Click += DelClick;
            this.notebookMainList.MouseDoubleClick += SelectDoubleClick;
        }

        private void SelectDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NotebookMainList.NotebookMain notebookMain =  this.notebookMainList.SelectedItem as NotebookMainList.NotebookMain;

            if (notebookMain != null)
            {
                MessageBox.Show(notebookMain.name);
            }
        }

        private void LoadNotebook()
        {
            // 首先清空所有的数据，防止有重复
            this.notebookMainList.Items.Clear();

            IDictionary<string, string> parameters = Request.getParameters();

            string resultStr = Http.Send(Request.baseUrl + Request.notebookMainList, "GET", parameters);
            NotebookMainList result = JsonConvert.DeserializeObject<NotebookMainList>(resultStr);

            foreach (NotebookMainList.NotebookMain noteBookMain in result.data)
            {
                this.notebookMainList.Items.Add(noteBookMain);
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            // 填写日记本名称
            Input input =  new Input();
            string content = "";
            input.confirm.Click += (object _sender, RoutedEventArgs _e) =>
            {
                content = input.input.Text;

                if (content.Length <= 0)
                    MessageBox.Show("请输入标题");
                else
                {
                    IDictionary<string, string> parameters = Request.getParameters();
                    parameters.Add("name", content);
                    Result result = JsonConvert.DeserializeObject<Result>(Http.Send(Request.baseUrl + Request.notebookCreate, "POST", parameters));

                    MessageBox.Show(result.message);

                    if (result.code == 1000)
                    {
                        input.Close();
                        LoadNotebook();
                    }
                }
            };

            input.ShowDialog();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            // 删除指定的数据
            NotebookMainList.NotebookMain notebookMain = this.notebookMainList.SelectedItem as NotebookMainList.NotebookMain;

            if (MessageBoxResult.Yes == MessageBox.Show("确认删除日记本《" + notebookMain.name + "》?", "确认", MessageBoxButton.YesNo))
            {
                IDictionary<string, string> parameters = Request.getParameters();
                parameters.Add("id", notebookMain.id.ToString());
                Result result = JsonConvert.DeserializeObject<Result>(Http.Send(Request.baseUrl + Request.notebookDel, "POST", parameters));

                MessageBox.Show(result.message);

                if (result.code == 1000)
                    LoadNotebook();
            }
        }
    }
}
