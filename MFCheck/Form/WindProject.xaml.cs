using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace MFCheck.Form
{
    /// <summary>
    /// WindProjectMain.xaml 的交互逻辑
    /// </summary>
    public partial class WindProject : Window
    {
        public WindProject(string projName)
        {
            InitializeComponent();

            statePanel.Visibility = Visibility.Hidden;
            Title = projName;

            ProjName = projName;

            m_Thread = new Thread(FileThread);
            m_Thread.Start();
        }

        public string ProjName { get; private set; }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            m_Thread.Abort();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            
            if (folderBrowser.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            m_strSelectPath = folderBrowser.SelectedPath;
            Title = ProjName + " " + m_strSelectPath;

            m_MayaList.Clear();
            m_MayaList = GetFileFullPath(m_strSelectPath, "*.ma");

            fileGrid.ItemsSource = m_MayaList;
        }


        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (m_MayaList.Count == 0)
            {
                BtnOpen_Click(sender, e);
            }

            if (m_MayaList.Count == 0)
            {
                MessageBox.Show("选择文件", "Error");
                return;
            }

            SetOperationEnable(false);
            m_Event.Set();
        }

        private void SetOperationEnable(bool enable)
        {
            btnOpen.IsEnabled = enable;
            btnCheck.IsEnabled = enable;
            statePanel.Visibility = enable == true ? Visibility.Hidden : Visibility.Visible;
        }

        private List<MayaInfo> GetFileFullPath(string folder, string extName)
        {
            List<MayaInfo> fileList = new List<MayaInfo>();

            DirectoryInfo root = new DirectoryInfo(folder);
            FileInfo[] fileInfos = root.GetFiles(extName);

            foreach (var file in fileInfos)
            {
                MayaInfo mayaInfo = new MayaInfo()
                {
                    Name = file.Name,
                    Path = file.DirectoryName,
                    FullName = file.FullName,
                };

                fileList.Add(mayaInfo);
            }

            DirectoryInfo[] dirInfos = root.GetDirectories();
            foreach(var dir in dirInfos)
            {
                List<MayaInfo> list = GetFileFullPath(dir.FullName, extName);
                if (list.Count>0)
                {
                    fileList = fileList.Concat(list).ToList();
                }
            }
            return fileList;
        }

        private void FileThread()
        {
            while(true)
            {
                m_Event.WaitOne();
                
                foreach (var file in m_MayaList)
                {
                    //检查文件名规范




                    MayaDocument document = new MayaDocument();
                    if (!document.Load(file.FullName))
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            txtNotice.Inlines.Add(document.Error);
                        }));
                        continue;
                    }


                    // 检查规范
                    List<string> roots = document.RootElements;
                    foreach (var root in roots)
                    {
                        MayaElement element = document.GetMayaElement(root);

                    }
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SetOperationEnable(true);
                }));
                m_Event.Reset();
            }
        }

        //检查文件命名
        private bool CheckFileName(string fileName)
        {
            App app = Application.Current as App;
            Project project = app.Manager.GetProject(ProjName);
            if (null == project)
            {
                return false;
            }


            return true;
        }


        private List<MayaInfo> m_MayaList = new List<MayaInfo>();
        private Thread m_Thread;
        private AutoResetEvent m_Event = new AutoResetEvent(false);
        private string m_strSelectPath;
    }



    class MayaInfo
    {
        //文件名
        public string Name { get; set; }

        //文件路径
        public string Path { get; set; }

        //全路径名
        public string FullName { get; set; }

        //Maya版本
        public string Product { get; set; }
    }
}
