﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MFCheck
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) 
        {
            base.OnStartup(e);

            //加载配置
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            //导出配置
        }

        public ProjectSet Manager { get; private set; } = new ProjectSet();
    }
}
