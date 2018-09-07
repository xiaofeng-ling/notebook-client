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
using Newtonsoft.Json;
using notebook.request;

namespace notebook
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int start = 0;
        private int limit = 100;

        private NotebookList.Notebook prevSelectedItems = null;

        public MainWindow()
        {
            InitializeComponent();

            Config config = Config.LoadConfig();
            GlobalVar.token = config.token;

            //if ("" == GlobalVar.token)
                new Login().ShowDialog();

            new NoteBookMain().ShowDialog();

            LoadNotebookPage();

            this.add.Click += this.AddClick;
            this.save.Click += this.SaveClickAsync;
            this.list.SelectionChanged += ListSelectionChanged;

            this.text.Text = GlobalVar.token;
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prevSelectedItems != null)
                SaveNotebookContent(prevSelectedItems, this.text.Text, false);

            NotebookList.Notebook notebook = (sender as ListBox).SelectedItem as NotebookList.Notebook;

            LoadNotebookContent(notebook.id);

            prevSelectedItems = notebook;
        }

        /// <summary>
        /// 加载日记本页
        /// </summary>
        private void LoadNotebookPage()
        {
            if (GlobalVar.notebook_id <= 0)
            {
                MessageBox.Show("请选择日记本！");
            }

            IDictionary<string, string> parameters =Request.getParameters();
            parameters.Add("notebook_id", GlobalVar.notebook_id.ToString());
            parameters.Add("start", start.ToString());
            parameters.Add("end", (start + limit).ToString());

            string resultStr = Http.Send(Request.baseUrl + Request.notebookList, "GET", parameters);

            NotebookList result = JsonConvert.DeserializeObject<NotebookList>(resultStr);

            foreach (NotebookList.Notebook noteBook in result.data)
            {
                this.list.Items.Add(noteBook);
            }

            MessageBox.Show(GlobalVar.notebook_id.ToString());
        }

        private void SaveClickAsync(object sender, RoutedEventArgs e)
        {
            SaveNotebookContent(this.list.SelectedItem as NotebookList.Notebook, this.text.Text);
        }

        /// <summary>
        /// 添加一页日记本的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClick(object sender, RoutedEventArgs e)
        {
            // 填写页的名称
            Input input = new Input();
            string content = "";
            input.confirm.Click += (object _sender, RoutedEventArgs _e) =>
            {
                content = input.input.Text;

                if (content.Length <= 0)
                    MessageBox.Show("请输入标题");
                else
                {
                    IDictionary<string, string> parameters = Request.getParameters();
                    parameters.Add("title", content);
                    parameters.Add("notebook_id", GlobalVar.notebook_id.ToString());
                    Result result = JsonConvert.DeserializeObject<Result>(Http.Send(Request.baseUrl + Request.notebookCreate, "POST", parameters));

                    MessageBox.Show(result.message);

                    if (result.code == 1000)
                    {
                        input.Close();
                        LoadNotebookPage();
                    }
                }
            };

            input.ShowDialog();

            //this.list.Items.Add();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
        }

        /// <summary>
        /// 加载日记本内容
        /// </summary>
        /// <param name="id"></param>
        private void LoadNotebookContent(int id)
        {
            IDictionary<string, string> parameters = Request.getParameters();

            LoadNotebookResult result = JsonConvert.DeserializeObject<LoadNotebookResult>(Http.Send(Request.baseUrl + Request.LoadNotebookContent + id.ToString(), "GET", parameters));

            if (result.code == 1000)
                this.text.Text = result.data.content;
        }

        /// <summary>
        /// 保存日记本内容
        /// </summary>
        /// <param name="notebook"></param>
        /// <param name="content"></param>
        private void SaveNotebookContent(NotebookList.Notebook notebook, string content, bool async = true)
        {
            long updated_at = Int64.Parse(Helper.GetTimeStamp());

            IDictionary<string, string> parameters = Request.getParameters();
            parameters.Add("id", notebook.id.ToString());
            parameters.Add("title", notebook.title);
            parameters.Add("content", content);
            parameters.Add("updated_at", (updated_at/1000).ToString());

            void callback(string responseText)
            {
                UpdateResult result = JsonConvert.DeserializeObject<UpdateResult>(responseText);

                if (result.code == 1000)
                    this.list.Dispatcher.Invoke(() =>
                    {
                        (this.list.SelectedItem as NotebookList.Notebook).updated_at = result.data.updated_at;
                    });
                    
            }

            string tempResult = "";
            if (async)
                #pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                Http.SendAsync(Request.baseUrl + Request.SaveNotebook, "POST", parameters, callback);
            else
            {
                tempResult = Http.Send(Request.baseUrl + Request.SaveNotebook, "POST", parameters);
                callback(tempResult);
            }
        }
    }

    // 全局变量类
    public class GlobalVar
    {
        // 登录过后的token
        public static string token = "";

        // 登录过后的用户id
        public static int user_id = 0;

        // 当前被选中的日记本id
        public static int notebook_id;
    }
}
