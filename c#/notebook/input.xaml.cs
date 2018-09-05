﻿using System;
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

namespace notebook
{
    /// <summary>
    /// input.xaml 的交互逻辑
    /// </summary>
    public partial class Input : Window
    {
        public Input()
        {
            InitializeComponent();

            //this.confirm.Click += ClickClose;
            this.cancel.Click += ClickClose;
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
