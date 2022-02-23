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
            ExportWizard exportWizard = new ExportWizard
            {
                Owner = this
            };
            exportWizard.ShowDialog();
        }

        //选择单元格
        private void FileGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                fileList.Add(new MayaInfo(file.FullName)
                {
                    CreateTime = file.CreationTime.ToString(),
                    ModifyTime = file.LastWriteTime.ToString(),
                });
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
                                        ModelNode node = new ModelNode
                                        {
                                            RootName = rootEle.Name
                                        };

                                        foreach (var child in rootEle.Children)
                                        {
                                            node.ChildName.Add(child.Name);
                                        }

                                        mayaAsset.Models.Add(node);
                                    }
                                }
                            }
                        }

                        if (!file.AddAsset(mayaAsset))
                        {
                            mayaAsset.Error = "无法识别资产类型";
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                txtNotice.Inlines.Add(mayaAsset.Error + mayaAsset.FullName);
                                txtNotice.Inlines.Add(new LineBreak());
                            }));
                        }
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

            if (!CurProject.TestCamName(mayaInfo.Cameras[0]))
            {
                return false;
            }

            //检查场景资产
            foreach (var asset in mayaInfo.SceneAssets)
            {
                if (!string.IsNullOrEmpty(asset.Error))
                {
                    return false;
                }

                foreach (var node in asset.Models)
                {
                    if (!CurProject.TestSceneName(node.RootName))
                    {
                        return false;
                    }
                }
            }


            //检查角色资产
            foreach (var asset in mayaInfo.CharAssets)
            {
                if (!string.IsNullOrEmpty(asset.Error))
                {
                    return false;
                }

                foreach (var node in asset.Models)
                {
                    if (!CurProject.TestCharName(node.RootName))
                    {
                        return false;
                    }

                    if (node.ChildName.Count != 3)
                    {
                        return false;
                    }

                    string rootName = node.RootName.Substring(0, node.RootName.Count() - 3);

                    string modName = rootName + "Mod";
                    string jointName = rootName + "Joint";
                    string controlName = rootName + "Ctrl";

                    foreach (var child in node.ChildName)
                    {
                        if (child != modName &&
                            child !=jointName &&
                            child != controlName)
                        {
                            return false;
                        }
                    }
                }
            }

            //检查道具资产
            foreach (var asset in mayaInfo.PropAssets)
            {
                if (!string.IsNullOrEmpty(asset.Error))
                {
                    return false;
                }

                foreach (var node in asset.Models)
                {
                    if (!CurProject.TestPropName(node.RootName))
                    {
                        return false;
                    }

                    if (node.ChildName.Count != 3)
                    {
                        return false;
                    }

                    string rootName = node.RootName.Substring(0, node.RootName.Count() - 3);

                    string modName = rootName + "Mod";
                    string jointName = rootName + "Joint";
                    string controlName = rootName + "Ctrl";

                    foreach (var child in node.ChildName)
                    {
                        if (child != modName &&
                            child != jointName &&
                            child != controlName)
                        {
                            return false;
                        }
                    }
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

            // 去除系统创建的节点
            if ("persp" == rootEle.Name || "top" == rootEle.Name ||
                "front" == rootEle.Name || "side" == rootEle.Name)
            {
                return false;
            }

            return CurProject.IsModName(rootEle.Name);
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
 
        //显示文件信息
        private void ShowFileInfo(MayaInfo mayaInfo)
        {
            if (null == mayaInfo)
            {
                return;
            }

            string placeholder = "                 ";

            List<Inline> result = new List<Inline>();

            result.Add(new Run("文件名称："));
            if (CurProject.TestFileName(mayaInfo.Name))
            {
                result.Add(new Run(mayaInfo.Name));
            }
            else
            {
                result.Add(new Run(mayaInfo.Name) { Foreground = Brushes.Red });
                result.Add(new LineBreak());
                result.Add(new Run(placeholder + "(文件名不正确)") { Foreground = Brushes.Red });
            }

            result.Add(new LineBreak());
            result.Add(new Run("相机列表："));
            if (mayaInfo.Cameras.Count == 0)
            {
                result.Add(new Run("无法找到相机") { Foreground = Brushes.Red });
            }
            else if (mayaInfo.Cameras.Count == 1)
            {
                result.Add(new Run(mayaInfo.Cameras[0]));
            }
            else
            {
                result.Add(new Run(mayaInfo.Cameras[0]));
                for (int i = 1; i < mayaInfo.Cameras.Count; ++i)
                {
                    result.Add(new LineBreak());
                    result.Add(new Run(placeholder + mayaInfo.Cameras[i]) { Foreground = Brushes.Red });
                }
            }

            // 场景
            result.Add(new LineBreak());
            result.Add(new Run("场景列表："));
            for (int i = 0; i < mayaInfo.SceneAssets.Count; ++i)
            {
                var sceneInfo = mayaInfo.SceneAssets[0];
                int curSceneCount = sceneInfo.Models.Count;
                for (int j = 0; j < curSceneCount; ++j)
                {
                    string sceneName = sceneInfo.Models[j].RootName;
                    Inline inline = null;
                    if (i == 0 && j == 0)
                    {
                        inline = new Run(sceneName);
                    }
                    else
                    {
                        result.Add(new LineBreak());
                        inline = new Run(placeholder + sceneName);
                    }

                    result.Add(inline);

                    if (!CurProject.TestSceneName(sceneName))
                    {
                        inline.Foreground = Brushes.Red;
                    }
                }
            }

            // 角色
            result.Add(new LineBreak());
            result.Add(new Run("角色列表："));
            for (int i = 0; i < mayaInfo.CharAssets.Count; ++i)
            {
                var charInfo = mayaInfo.CharAssets[0];
                int curCharCount = charInfo.Models.Count;
                for (int j = 0; j < curCharCount; ++j)
                {
                    string charName = charInfo.Models[j].RootName;
                    Inline inline = null;
                    if (i == 0 && j == 0)
                    {
                        inline = new Run(charName);
                    }
                    else
                    {
                        result.Add(new LineBreak());
                        inline = new Run(placeholder + charName);
                    }

                    result.Add(inline);

                    if (!CurProject.TestCharName(charName))
                    {
                        inline.Foreground = Brushes.Red;
                    }

                    if (charInfo.Models[j].ChildName.Count != 3)
                    {
                        inline.Foreground = Brushes.Red;
                    }

                    string temp = charName.Substring(0, charName.Count() - 3);
                    string modName = temp + "Mod";
                    string jointName = temp + "Joint";
                    string controlName = temp + "Ctrl";

                    foreach (var name in charInfo.Models[j].ChildName)
                    {
                        Inline childLine = new Run(placeholder + "      " + name);                        
                        if (name != modName && 
                            name != jointName && 
                            name != controlName)
                        {
                            childLine.Foreground = Brushes.Red;
                        }
                        result.Add(new LineBreak());
                        result.Add(childLine);
                    }
                }
            }

            // 角色
            result.Add(new LineBreak());
            result.Add(new Run("道具列表："));
            for (int i = 0; i < mayaInfo.PropAssets.Count; ++i)
            {
                var info = mayaInfo.PropAssets[0];
                int modCount = info.Models.Count;
                for (int j = 0; j < modCount; ++j)
                {
                    string rootName = info.Models[j].RootName;
                    Inline inline = null;
                    if (i == 0 && j == 0)
                    {
                        inline = new Run(rootName);
                    }
                    else
                    {
                        result.Add(new LineBreak());
                        inline = new Run(placeholder + rootName);
                    }

                    result.Add(inline);

                    if (!CurProject.TestPropName(rootName))
                    {
                        inline.Foreground = Brushes.Red;
                    }

                    if (info.Models[j].ChildName.Count != 3)
                    {
                        inline.Foreground = Brushes.Red;
                    }

                    string temp = rootName.Substring(0, rootName.Count() - 3);
                    string modName = temp + "Mod";
                    string jointName = temp + "Joint";
                    string controlName = temp + "Ctrl";

                    foreach (var name in info.Models[j].ChildName)
                    {
                        Inline childLine = new Run(placeholder + "      " + name);
                        if (name != modName &&
                            name != jointName &&
                            name != controlName)
                        {
                            childLine.Foreground = Brushes.Red;
                        }
                        result.Add(new LineBreak());
                        result.Add(childLine);
                    }
                }
            }

            txtNotice.Inlines.Clear();
            foreach (var inline in result)
            {
                txtNotice.Inlines.Add(inline);
            }
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

    //资产类型
    class MAssetType
    {
        public static readonly string Unknown = "unknown";
        public static readonly string Character = "characters";
        public static readonly string Scene = "environment";
        public static readonly string Prop = "props";
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

            if (FullName.Contains(MAssetType.Character))
            {
                AssetType = MAssetType.Character;
            }
            else if (FullName.Contains("environment"))
            {
                AssetType = MAssetType.Scene;
            }
            else if (FullName.Contains("props"))
            {
                AssetType = MAssetType.Prop;
            }
            else
            {
                AssetType = MAssetType.Unknown;
            }
        }

        //文件名
        public string Name { get; }

        //全路径名
        public string FullName { get; }

        //文件创建时间
        public string CreateTime { get; set; }

        //文件修改时间
        public string ModifyTime { get; set; }

        //资产类型
        public string AssetType { get; }

        //加载文件产生的错误
        public string Error { get; set; }

        //相机列表
        public List<string> Cameras { get; } = new List<string>();

        //模型列表
        public List<ModelNode> Models { get; } = new List<ModelNode>();

        //场景资产
        public List<MayaInfo> SceneAssets { get; } = new List<MayaInfo>();

        //角色资产
        public List<MayaInfo> CharAssets { get; } = new List<MayaInfo>();

        //道具资产
        public List<MayaInfo> PropAssets { get; } = new List<MayaInfo>();

        //添加资产
        public bool AddAsset(MayaInfo assetInfo)
        {
            if (assetInfo.AssetType == MAssetType.Scene)
            {
                SceneAssets.Add(assetInfo);
            }
            else if (assetInfo.AssetType == MAssetType.Character)
            {
                CharAssets.Add(assetInfo);
            }
            else if (assetInfo.AssetType == MAssetType.Prop)
            {
                PropAssets.Add(assetInfo);
            }
            else
            {
                return false;
            }

            return true;
        }

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

            Error = "";
            Cameras.Clear();
            Models.Clear();
            SceneAssets.Clear();
            CharAssets.Clear();
            PropAssets.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
