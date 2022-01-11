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

namespace MFCheck.Form
{
    /// <summary>
    /// WindNewProject.xaml 的交互逻辑
    /// </summary>
    public partial class WindNewProject : Window
    {
        public WindNewProject()
        {
            InitializeComponent();
        }

        private void BtnAddProp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButDelProp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string propName = txtProName.Text;
            if (string.IsNullOrEmpty(propName))
            {
                txtProName.SetValue(StyleProperty, FindResource("errTextBox"));
            }
            else
            {
                txtProName.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            string propDesc = txtProDesc.Text;
            



        }
    }
}
