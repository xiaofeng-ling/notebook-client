﻿using System;
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
        private bool isEnd = false;

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
            this.delete.Click += this.DeleteClick;
            this.modify.Click += this.ModifyClick;

            this.list.SelectionChanged += this.ListSelectionChanged;

            // 在可视化树创建完成后才能够找到子节点
            this.Loaded += (object sender, RoutedEventArgs e) =>
            {
                ScrollViewer scrollViewer = FindVisualFirstChild<ScrollViewer>(this.list);

                if (scrollViewer != null)
                    scrollViewer.ScrollChanged += ScrollChanged;
            };

            // 定时保存，每60秒保存一次
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 10 * 1000,     // 每60秒执行一次
                Enabled = true
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(AutoSave);
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (scroll.VerticalOffset >= scroll.ScrollableHeight)
                LoadNotebookPage();
        }

        private void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetChild(this.list, 1) as ScrollViewer;
        }

        public void AutoSave(object sender, System.Timers.ElapsedEventArgs e)
        {
            NotebookList.Notebook notebook = GetSelectedNotebook();
            string text = "";

            this.text.Dispatcher.Invoke(() =>
            {
                text = this.text.Text;
            });

            if (notebook != null && text != "")
                SaveNotebookContent(notebook, text);

            // 自动保存，消息提示
            //MessageBox.Show("保存成功");
        }

        private void ModifyClick(object sender, RoutedEventArgs e)
        {
            NotebookList.Notebook notebook = GetSelectedNotebook();

            Input input = new Input();
            string content = "";
            input.input.Text = notebook.title;

            input.confirm.Click += (object _sender, RoutedEventArgs _e) =>
            {
                content = input.input.Text;

                if (content.Length <= 0)
                    MessageBox.Show("请输入标题");
                else
                {
                    IDictionary<string, string> parameters = Request.getParameters();
                    parameters.Add("id", notebook.id.ToString());
                    parameters.Add("title", notebook.title);

                    Result result = JsonConvert.DeserializeObject<Result>(Http.Send(Request.baseUrl + Request.ModifyTitleNotebook, "POST", parameters));

                    if (result.code == 1000)
                    {
                        // 更新名字
                        notebook.title = content;
                        this.list.Items.Refresh();
                        input.Close();
                    }
                }
            };

            input.ShowDialog();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            NotebookList.Notebook notebook = GetSelectedNotebook();

            IDictionary<string, string> parameters = Request.getParameters();
            parameters.Add("id", notebook.id.ToString());

            Result result = JsonConvert.DeserializeObject<Result>(Http.Send(Request.baseUrl + Request.DeleteNotebook, "POST", parameters));

            if (result.code == 1000)
            {
                prevSelectedItems = null;
                this.list.Items.Remove(notebook);

                // 减去1的原因是为了方便后续的分页加载，防止加载断层
                start--;
            }
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prevSelectedItems != null)
                SaveNotebookContent(prevSelectedItems, this.text.Text, false);

            NotebookList.Notebook notebook = GetSelectedNotebook();

            if (notebook != null)
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

            if (isEnd)
                return;

            IDictionary<string, string> parameters = Request.getParameters();
            parameters.Add("notebook_id", GlobalVar.notebook_id.ToString());
            parameters.Add("start", start.ToString());
            parameters.Add("end", (start + limit).ToString());

            string resultStr = Http.Send(Request.baseUrl + Request.notebookList, "GET", parameters);

            NotebookList result = JsonConvert.DeserializeObject<NotebookList>(resultStr);

            foreach (NotebookList.Notebook noteBook in result.data)
            {
                this.list.Items.Add(noteBook);
                start++;
            }

            // 已经结束了
            if (result.data.Count == 0)
                isEnd = true;

            this.list.Items.Refresh();
        }

        private void SaveClickAsync(object sender, RoutedEventArgs e)
        {
            SaveNotebookContent(GetSelectedNotebook(), this.text.Text);
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

                    if (result.code == 1000)
                    {
                        input.Close();
                        LoadNotebookPage();

                        // 选中新增的一页
                        this.list.SelectedIndex = this.list.Items.Count - 1;
                    }
                }
            };

            input.ShowDialog();
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

            if (notebook is null || content == "")
                return;

            IDictionary<string, string> parameters = Request.getParameters();
            parameters.Add("id", notebook.id.ToString());
            parameters.Add("title", notebook.title);
            parameters.Add("content", content);
            parameters.Add("updated_at", (updated_at / 1000).ToString());

            void callback(string responseText)
            {
                UpdateResult result = JsonConvert.DeserializeObject<UpdateResult>(responseText);

                if (result.code == 1000)
                    this.list.Dispatcher.Invoke(() =>
                    {
                        notebook.updated_at = result.data.updated_at;
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

        private NotebookList.Notebook GetSelectedNotebook()
        {
            NotebookList.Notebook notebook = null;

            this.list.Dispatcher.Invoke(() =>
            {
                notebook = this.list.SelectedItem as NotebookList.Notebook;
            });

            return notebook;
        }

        /// <summary>
        /// 递归查找第一个子节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T FindVisualFirstChild<T>(DependencyObject obj) where T : DependencyObject
         {
            List<T> list = FindVisualChildList<T>(obj);

            return list != null && list.Count > 0 
                ? list[0] : null;
         }

        /// <summary>
        /// 递归查找子节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<T> FindVisualChildList<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                List<T> TList = new List<T> { };
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        TList.Add((T)child);
                        List<T> childOfChildren = FindVisualChildList<T>(child);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                    else
                    {
                        List<T> childOfChildren = FindVisualChildList<T>(child);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                }
                return TList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
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
