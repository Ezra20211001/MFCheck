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
        public ObservableCollection<Project> ProjectSet { get; set; } = new ObservableCollection<Project>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ProjectSet.Add(new Project() { Name = "NTZJ", Description = "逆天战绩" });
            ProjectSet.Add(new Project() { Name = "NTZJ1", Description = "逆天战绩" });
            ProjectSet.Add(new Project() { Name = "NTZJ2", Description = "逆天战绩" });
            ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
            //ProjectSet.Add(new Project() { Name = "NTZJ3", Description = "逆天战绩" });
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About fmAbout = new About();
            fmAbout.Owner = this;
            fmAbout.ShowDialog();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            WindNewProject windNewProject = new WindNewProject();
            windNewProject.Owner = this;
            windNewProject.ShowDialog();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //            WindProject windProject = new WindProject();
            //          windProject.Owner = this;
            //         windProject.ShowDialog();

            Window1 wind = new Window1();
            wind.Owner = this;
            wind.Show();
        }

        private void ProjList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
