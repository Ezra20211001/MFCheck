using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using System.Windows.Media;

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
            CurProject = project;

            statePanel.Visibility = Visibility.Hidden;
            Title = CurProject.Name;

            m_Thread = new Thread(ReadThread);
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
            Title = CurProject.Name + " " + m_strSelectPath;

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

            // 重置文件列表
            foreach (var file in m_MayaList)
            {
                file.ResetStatus();
            }

            m_Event.Set();
        }

        // 导出文件
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //if (m_Event.)
        }

        //选择单元格
        private void FileGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowFileInfo((MayaInfo)fileGrid.SelectedItem);
        }

        private void SetOperationEnable(bool enable)
        {
            btnOpen.IsEnabled = enable;
            btnCheck.IsEnabled = enable;
            btnExport.IsEnabled = enable;
            statePanel.Visibility = enable == true ? Visibility.Hidden : Visibility.Visible;
        }

        //遍历文件夹读取文件列表
        private List<MayaInfo> GetFileFullPath(string folder, string extName)
        {
            List<MayaInfo> fileList = new List<MayaInfo>();

            DirectoryInfo root = new DirectoryInfo(folder);
            FileInfo[] fileInfos = root.GetFiles(extName);

            foreach (var file in fileInfos)
            {
                fileList.Add(new MayaInfo(file.FullName));
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

        //读取线程
        private void ReadThread()
        {
            while(true)
            {
                m_Event.WaitOne();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtNotice.Inlines.Clear();
                }));


                Dictionary<string, MayaInfo> AssetDic = new Dictionary<string, MayaInfo>();

                foreach (var file in m_MayaList)
                {
                    file.Status = "... ...";

                    MayaDocument document = new MayaDocument();
                    if (!document.Load(file.FullName))
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            txtNotice.Inlines.Add(document.Error);
                            txtNotice.Inlines.Add(new LineBreak());
                        }));

                        file.Error = document.Error;
                        file.Status = "失败";
                        continue;
                    }

                    //加载引用文件
                    for (int i = 0; i < document.MayaReferenceList.Count; ++i)
                    {
                        MayaInfo mayaAsset = null;

                        MayaReference mayaRef = document.MayaReferenceList[i];
                        if (AssetDic.ContainsKey(mayaRef.ReferenceNode))
                        {
                            mayaAsset = AssetDic[mayaRef.ReferenceNode];
                        }
                        else
                        {
                            mayaAsset = new MayaInfo(mayaRef.ReferenceFile);
                            AssetDic[mayaRef.ReferenceNode] = mayaAsset;

                            MayaDocument assetDoc = new MayaDocument();
                            mayaRef.Document = assetDoc;

                            if (!assetDoc.Load(mayaRef.ReferenceFile))
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    txtNotice.Inlines.Add(assetDoc.Error);
                                    txtNotice.Inlines.Add(new LineBreak());
                                }));

                                mayaAsset.Error = assetDoc.Error;
                            }
                            else
                            {
                                foreach (var rootEle in assetDoc.RootElements)
                                {
                                    if (IsModel(rootEle))
                                    {
                                        ModelNode node = new ModelNode();
                                        node.RootName = rootEle.Name;
                                        foreach (var child in rootEle.Children)
                                        {
                                            node.ChildName.Add(child.Name);
                                        }
                                        mayaAsset.Models.Add(node);
                                    }
                                }
                            }
                        }

                        file.Assets.Add(mayaAsset);
                    }

                    //收集摄像机
                    foreach (var rootEle in document.RootElements)
                    {
                        if (IsCamera(rootEle))
                        {
                            file.Cameras.Add(rootEle.Name);
                        }
                    }

                    //验证命名
                    if (TestNaming(file))
                    {
                        file.Status = "成功";
                    }
                    else
                    {
                        file.Status = "失败";
                    }
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SetOperationEnable(true);
                }));
                m_Event.Reset();
            }
        }

        // 验证命名
        private bool TestNaming(MayaInfo mayaInfo)
        {
            if (mayaInfo.Cameras.Count() != 1)
            {
                return false;
            }

            if (!CurProject.TestingCamName(mayaInfo.Cameras[0]))
            {
                return false;
            }

            //检查模型命名
            foreach(var asset in mayaInfo.Assets)
            {
                if (!string.IsNullOrEmpty(asset.Error))
                {
                    return false;
                }

                foreach(var node in asset.Models)
                {
                    if (!CurProject.TestingModName(node.RootName))
                    {
                        return false;
                    }

                    //检查模型、骨骼、控制器
                }
            }

            return true;
        }

        //是否是模型
        private bool IsModel(MayaElement rootEle)
        {
            if (rootEle.Type != MayaElementType.METransform)
            {
                return false;
            }

            //必须有模型、骨骼、控制器，三个部分组成
            if (rootEle.Children.Count() != 3)
            {
                return false;
            }

            return false;
        }

        //是否是相机
        private bool IsCamera(MayaElement rootEle)
        {
            // 去除系统创建的节点
            if ("persp" == rootEle.Name || "top" == rootEle.Name ||
                "front" == rootEle.Name || "side" == rootEle.Name)
            {
                return false;
            }

            if (rootEle.Type != MayaElementType.METransform)
            {
                return false;
            }

            if (rootEle.Children.Count() != 1)
            {
                return false;
            }

            if (rootEle.Children[0].Type != MayaElementType.MECamera)
            {
                return false;
            }

            return true;
        }

        //文件夹从错误
        private void InvokeFileError(MayaInfo info, string desc)
        {
            string error = desc + ": " + info.Name +"\n";
            Dispatcher.BeginInvoke(new Action(() =>
            {
                info.Status = "失败";
                txtNotice.Inlines.Add(error);
            }));
        }
 
        //显示文件信息
        private void ShowFileInfo(MayaInfo mayaInfo)
        {
            if (null == mayaInfo)
            {
                return;
            }

            txtNotice.Inlines.Clear();

            // 显示文件名
            txtNotice.Inlines.Add("文件名称：");
            if (CurProject.TestingFileName(mayaInfo.Name))
                txtNotice.Inlines.Add(mayaInfo.Name);
            else
                txtNotice.Inlines.Add(new Run(mayaInfo.Name) { Foreground = Brushes.Red });
            txtNotice.Inlines.Add(new LineBreak());

            // 显示相机名称
            txtNotice.Inlines.Add("相机名称：");
            int cameraCount = mayaInfo.Cameras.Count();
            if (cameraCount == 0)
            {
                txtNotice.Inlines.Add(new Run("无法找到相机") { Foreground = Brushes.Red });
            }
            else if (cameraCount == 1)
            {
                if (CurProject.TestingCamName(mayaInfo.Cameras[0]))
                    txtNotice.Inlines.Add(mayaInfo.Cameras[0]);
                else
                    txtNotice.Inlines.Add(new Run(mayaInfo.Cameras[0]) { Foreground = Brushes.Red });
            }
            else
            {
                txtNotice.Inlines.Add(new Run(mayaInfo.Cameras[0]) { Foreground = Brushes.Red });
                for (int i = 1; i < cameraCount; ++i)
                {
                    string showInfo = "          " + mayaInfo.Cameras[i];
                    txtNotice.Inlines.Add(new Run(showInfo) { Foreground = Brushes.Red });
                }
            }
            txtNotice.Inlines.Add(new LineBreak());

            // 显示模型名称
            txtNotice.Inlines.Add("模型名称：");

        }

        private ObservableCollection<MayaInfo> m_MayaList;
        private Thread m_Thread;
        private AutoResetEvent m_Event = new AutoResetEvent(false);
        private string m_strSelectPath;
        private Project CurProject { set; get; }

    }

    //模型节点
    class ModelNode
    {
        public string RootName { get; set; }
        public List<string> ChildName { get; } = new List<string>();
    }

    //文件信息
    class MayaInfo : INotifyPropertyChanged
    {
        private string m_Status;
        private string m_Prodect;

        public MayaInfo(string fullName)
        {
            FullName = fullName;
            Name = Path.GetFileNameWithoutExtension(fullName);
        }

        //文件名
        public string Name { get; }

        //全路径名
        public string FullName { get; }

        //加载文件产生的错误
        public string Error { get; set; }

        //相机列表
        public List<string> Cameras { get; } = new List<string>();

        //模型列表
        public List<ModelNode> Models { get; } = new List<ModelNode>();

        //资产信息
        public List<MayaInfo> Assets { get; } = new List<MayaInfo>();

        //Maya 版本
        public string Product
        {
            get { return m_Prodect; }
            set
            {
                m_Prodect = value;
                if (null != PropertyChanged)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Product"));
            }
        }

        //文件状态
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

        public void ResetStatus()
        {
            Product = "";
            Status = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
