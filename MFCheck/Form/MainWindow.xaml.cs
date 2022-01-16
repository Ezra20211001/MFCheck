using MFCheck.Form;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
