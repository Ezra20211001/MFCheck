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
        private Project m_Project = new Project();

        public WindNewProject()
        {
            InitializeComponent();

            propGrid.ItemsSource = m_Project.PropertySet;
            combPropType.ItemsSource = Enum.GetValues(typeof(PropType));
        }

        //改变选择
        private void PropGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == e.AddedItems || e.AddedItems.Count == 0)
            {
                return;
            }

            Property property = e.AddedItems[0] as Property;
            txtPropName.Text = property.Name;
            combPropType.Text = property.Type.ToString();
            txtValue.Text = property.Value;
            txtDesc.Text = property.Desc;
        }

        private void BtnAddProp_Click(object sender, RoutedEventArgs e)
        {
            //检查属性名
            string propName = txtPropName.Text;
            if (string.IsNullOrEmpty(propName))
            {
                MessageBox.Show("输入属性名!", "Error");
                txtPropName.SetValue(StyleProperty, FindResource("errTextBox"));
                return;
            }
            else
            {
                txtPropName.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查属性类型
            PropType propType;
            if (string.IsNullOrEmpty(combPropType.Text))
            {
                MessageBox.Show("选择类型!", "Error");
                combPropType.SetValue(StyleProperty, FindResource("errComboBox"));
                return;
            }
            else
            {
                propType = (PropType)Enum.Parse(typeof(PropType), combPropType.Text);
                combPropType.SetValue(StyleProperty, FindResource("norComboBox"));
            }

            //检查属性值
            string propVal = txtValue.Text;
            if (propType == PropType.Const || propType == PropType.Enum)
            {
                if (string.IsNullOrEmpty(propVal))
                {
                    txtValue.SetValue(StyleProperty, FindResource("errTextBox"));
                    MessageBox.Show("属性必须赋值!", "Error");
                    return;
                }
            }
            
            txtValue.SetValue(StyleProperty, FindResource("norTextBox"));

            Property property = m_Project.AddProperty(propName);
            if (null == property)
            {
                MessageBox.Show("属性已经存在!", "Error");
                return;
            }

            property.Name = propName;
            property.Type = propType;
            property.Value = propVal;
            property.Desc = txtDesc.Text;
            property.ReadOnley = false;
            //刷新列表
            propGrid.ItemsSource = m_Project.PropertySet;
        }

        private void ButDelProp_Click(object sender, RoutedEventArgs e)
        {
            Property property = m_Project.GetProperty(txtPropName.Text);
            if (null == property)
            {
                MessageBox.Show("没有此属性!", "Error");
                return;
            }
            
            if (property.ReadOnley)
            {
                MessageBox.Show("当前属性不可被删除!", "Error");
                return;
            }

            m_Project.RemoveProperty(txtPropName.Text);

            //刷新列表
            propGrid.ItemsSource = m_Project.PropertySet;
        }

        private void ButModifyProp_Click(object sender, RoutedEventArgs e)
        {
            Property property = m_Project.GetProperty(txtPropName.Text);
            if (null == property)
            {
                MessageBox.Show("没有此属性!", "Error");
                return;
            }

            if (property.ReadOnley)
            {
                MessageBox.Show("当前属性不可被修改!", "Error");
                return;
            }

            //检查属性类型
            PropType propType;
            if (string.IsNullOrEmpty(combPropType.Text))
            {
                MessageBox.Show("选择类型!", "Error");
                combPropType.SetValue(StyleProperty, FindResource("errComboBox"));
                return;
            }
            else
            {
                propType = (PropType)Enum.Parse(typeof(PropType), combPropType.Text);
                combPropType.SetValue(StyleProperty, FindResource("norComboBox"));
            }

            //检查属性值
            string propVal = txtValue.Text;
            if (propType == PropType.Const || propType == PropType.Enum)
            {
                if (string.IsNullOrEmpty(propVal))
                {
                    txtValue.SetValue(StyleProperty, FindResource("errTextBox"));
                    MessageBox.Show("属性必须赋值!", "Error");
                    return;
                }
            }

            txtValue.SetValue(StyleProperty, FindResource("norTextBox"));

            property.Type = propType;
            property.Value = propVal;
            property.Desc = txtDesc.Text;

            //刷新列表
            propGrid.ItemsSource = m_Project.PropertySet;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string propName = txtProjName.Text;
            string propDesc = txtProDesc.Text;
            string fileNaming = txtFileNaming.Text;
            string modNaming = txtModNaming.Text;
            string camNaming = txtCamNaming.Text;

            if (string.IsNullOrEmpty(propName))
            {
                txtProjName.SetValue(StyleProperty, FindResource("errTextBox"));
                MessageBox.Show("输入项目名称", "Error");
                return;
            }
            else
            {
                Project project = (Application.Current as App).Manager.GetProject(propName);
                if (null != project)
                {
                    txtProjName.SetValue(StyleProperty, FindResource("errTextBox"));
                    MessageBox.Show("已经存在相同的项目名", "Error");
                    return;
                }

                txtProjName.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查文件命名
            if (!string.IsNullOrEmpty(fileNaming))
            {
                string[] result = fileNaming.Split('_');
                foreach(var prop in result)
                {
                    if (!m_Project.FindProperty(prop))
                    {
                        MessageBox.Show("无法找到属性:" + prop, "Error");
                        txtFileNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtFileNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            if (!string.IsNullOrEmpty(modNaming))
            {
                string[] result = modNaming.Split('_');
                foreach (var prop in result)
                {
                    if (!m_Project.FindProperty(prop))
                    {
                        MessageBox.Show("无法找到属性:" + prop, "Error");
                        txtModNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtModNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            if (!string.IsNullOrEmpty(camNaming))
            {
                string[] result = camNaming.Split('_');
                foreach (var prop in result)
                {
                    if (!m_Project.FindProperty(prop))
                    {
                        MessageBox.Show("无法找到属性:" + prop, "Error");
                        txtCamNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtCamNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            Property property = m_Project.GetProperty("Project");
            property.Value = propName;
            property.Desc = propDesc;

            m_Project.Name = propName;
            m_Project.Description = propDesc;
            m_Project.FileNaming = fileNaming;
            m_Project.ModNaming = modNaming;
            m_Project.CamNaming = camNaming;

            App app = (Application.Current as App);
            app.Manager.AddProject(m_Project);

            Close();
        }
    }
}
