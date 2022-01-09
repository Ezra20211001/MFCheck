using MFCheck.Form;
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
            WindProject windProject = new WindProject();

            windProject.Owner = this;
            windProject.ShowDialog();

//            Uri uri = pageProject.Source;
//            pageProject.Source = new Uri("/MFCheck;component/Form/PageEmptyProject.xaml", UriKind.Relative);
        }
    }
}
