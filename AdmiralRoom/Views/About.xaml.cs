﻿using System.Windows.Controls;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }
        public static string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
