using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MFCheck.Form
{
    /// <summary>
    /// WindProjectMain.xaml 的交互逻辑
    /// </summary>
    public partial class WindProject : Window
    {
        public WindProject(Project project)
        {
            InitializeComponent();
            m_CurProject = project;

            statePanel.Visibility = Visibility.Hidden;
            Title = m_CurProject.Name;

            m_Thread = new Thread(FileThread);
            m_Thread.Start();
        }


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
            Title = m_CurProject.Name + " " + m_strSelectPath;

            m_MayaList = new ObservableCollection<MayaInfo>(GetFileFullPath(m_strSelectPath, "*.ma"));
            fileGrid.ItemsSource = m_MayaList;
        }


        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (null == m_MayaList || m_MayaList.Count == 0)
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
                    file.Status = "... ...";

                    //检查文件名规范
                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);
                    if (!CheckFileNaming(fileName))
                    {
                        InvokeFileError(file, "文件命名错误");
                        continue;
                    }

                    MayaDocument document = new MayaDocument();
                    if (!document.Load(file.FullName))
                    {
                        InvokeFileError(file, document.Error);
                        continue;
                    }

                    // 检查规范
                    bool bSucceed = true;
                    foreach (var root in document.RootElements)
                    {
                        MayaElement rootEle = document.GetMayaElement(root);

                        //检查模型命名规范
                        if (!CheckModNaming(rootEle))
                        {
                            bSucceed = false;
                            InvokeFileError(file, "模型命名错误");
                            break;
                        }

                        //检查相机命名规范
                        if (!CheckCamNaming(rootEle))
                        {
                            bSucceed = false;
                            InvokeFileError(file, "相机命名错误");
                            break;
                        }
                    }

                    if (bSucceed)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            file.Status = "成功";
                        }));
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
        private bool CheckFileNaming(string fileName)
        {
            string[] nameSplit = fileName.Split('_');
            string[] config = m_CurProject.FileNaming.Split('_');
            if (nameSplit.Count() != config.Count())
            {
                return false;
            }
            
            for (int i = 0; i < config.Count(); ++i)
            {
                Property property = m_CurProject.GetProperty(config[i]);
                if (null == property)
                {
                    MessageBox.Show("配置错误", "Error");
                }

                if (!property.Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        //检查模型命名
        private bool CheckModNaming(MayaElement rootEle)
        {
            if (string.IsNullOrEmpty(m_CurProject.FileNaming))
            {
                return true;
            }

            if (rootEle.Type != MayaType.MTransform)
            {
                return true;
            }

            string modName;
            string[] modNameList = rootEle.NodeName.Split(':');
            if (modNameList.Count() > 0)
            {
                modName = modNameList[modNameList.Count() - 1];
            }
            else
            {
                modName = rootEle.NodeName;
            }

            string[] nameSplit = modName.Split('_');
            string[] config = m_CurProject.FileNaming.Split('_');

            // 属性数量不一致
            if (nameSplit.Count() != config.Count())
            {
                return true;
            }

            for (int i = 0; i < config.Count(); ++i)
            {
                Property property = m_CurProject.GetProperty(config[i]);
                if (null == property)
                {
                    MessageBox.Show("配置错误", "Error");
                }

                if (!property.Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        //检查相机命令
        private bool CheckCamNaming(MayaElement rootEle)
        {
            if (rootEle.Type != MayaType.MRefernce)
            {
                return true;
            }

            string modName;
            string[] modNameList = rootEle.NodeName.Split(':');
            if (modNameList.Count() > 0)
            {
                modName = modNameList[modNameList.Count() - 1];
            }
            else
            {
                modName = rootEle.NodeName;
            }

            string[] nameSplit = modName.Split('_');
            string[] config = m_CurProject.FileNaming.Split('_');
            if (nameSplit.Count() != config.Count())
            {
                return true;
            }

            for (int i = 0; i < config.Count(); ++i)
            {
                Property property = m_CurProject.GetProperty(config[i]);
                if (null == property)
                {
                    MessageBox.Show("配置错误", "Error");
                }

                if (!property.Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        //文件夹从错误
        private void InvokeFileError(MayaInfo info, string desc)
        {
            string error = desc + "：" + info.Name +"\n";
            Dispatcher.BeginInvoke(new Action(() =>
            {
                info.Status = "失败";
                txtNotice.Inlines.Add(error);
            }));
        }

        private ObservableCollection<MayaInfo> m_MayaList;
        private Thread m_Thread;
        private AutoResetEvent m_Event = new AutoResetEvent(false);
        private string m_strSelectPath;
        private Project m_CurProject;
    }


    class MayaInfo : INotifyPropertyChanged
    {
        //文件名
        public string Name { get; set; }

        //文件路径
        public string Path { get; set; }

        //全路径名
        public string FullName { get; set; }

        //Maya版本
        public string Product { get; set; }

        //文件状态
        private string m_Status;
        public string Status
        {
            get { return m_Status; }
            set
            {
                m_Status = value;
                if (null != PropertyChanged)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
