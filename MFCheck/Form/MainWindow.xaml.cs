using MFCheck.Form;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MFCheck
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ProjList.ItemsSource = (Application.Current as App).ProjectList.ToList();
            SelectionInfo.Visibility = Visibility.Hidden;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About fmAbout = new About
            {
                Owner = this
            };
            fmAbout.ShowDialog();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            WindNewProject windNewProject = new WindNewProject(new Project())
            {
                Owner = this
            };
            windNewProject.ShowDialog();
            ProjList.ItemsSource = (Application.Current as App).ProjectList.ToList();
        }

        private void ProjList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == ProjList.SelectedItem)
            {
                SelectionInfo.Visibility = Visibility.Hidden;
            }
            else
            {
                SelectionInfo.Visibility = Visibility.Visible;
            }
        }

        private void BtnSel_Click(object sender, RoutedEventArgs e)
        {
            if (ProjList.SelectedItems.Count == 0)
            {
                MessageBox.Show("先选择一个项目", "Error");
                return;
            }

            Project project = ProjList.SelectedItems[0] as Project;

            WindProject windProject = new WindProject(project)
            {
                Owner = this
            };
            windProject.ShowDialog();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProjList.SelectedItems.Count == 0)
            {
                MessageBox.Show("先选择一个项目", "Error");
                return;
            }

            Project project = ProjList.SelectedItems[0] as Project;
            WindNewProject windNewProject = new WindNewProject(project)
            {
                Owner = this
            };
            windNewProject.ShowDialog();
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (ProjList.SelectedItems.Count == 0)
            {
                MessageBox.Show("先选择一个项目", "Error");
                return;
            }

            Project project = ProjList.SelectedItems[0] as Project;
            (Application.Current as App).RemoveProject(project.Name);
            ProjList.ItemsSource = (Application.Current as App).ProjectList.ToList();
        }
    }
}
