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
using System.Windows.Forms;
using System.IO;

namespace MFCheck.Form
{
    /// <summary>
    /// WindProjectMain.xaml 的交互逻辑
    /// </summary>
    public partial class WindProject : Window
    {
        public WindProject()
        {
            InitializeComponent();
        }

        public string ProjectName { set; get; }

        protected override void OnSourceInitialized(EventArgs e)
        {
            Title = ProjectName;
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            
            if (folderBrowser.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            m_strSelectPath = folderBrowser.SelectedPath;
            Title = ProjectName + " " + m_strSelectPath;

            m_FileList = GetFileFullPath(m_strSelectPath, "*.ma");

            Console.WriteLine(m_FileList.Count);
        }

        private List<string> GetFileFullPath(string folder, string extName)
        {
            List<string> fileList = new List<string>();

            DirectoryInfo root = new DirectoryInfo(folder);
            FileInfo[] fileInfos = root.GetFiles(extName);

            foreach (var file in fileInfos)
            {
                fileList.Add(file.FullName);
            }

            DirectoryInfo[] dirInfos = root.GetDirectories();
            foreach(var dir in dirInfos)
            {
                List<string> list = GetFileFullPath(dir.FullName, extName);
                if (list.Count>0)
                {
                    fileList = fileList.Concat(list).ToList();
                }
            }
            return fileList;
        }

        private List<string> m_FileList;
        private string m_strSelectPath;
    }
}
