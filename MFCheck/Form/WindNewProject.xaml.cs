using System;
using System.Windows;
using System.Windows.Controls;

namespace MFCheck.Form
{
    /// <summary>
    /// WindNewProject.xaml 的交互逻辑
    /// </summary>
    public partial class WindNewProject : Window
    {
        private Project CurProject { get; set; }

        public WindNewProject(Project project)
        {
            InitializeComponent();
            CurProject = project;

            propGrid.ItemsSource = CurProject.PropertySet;
            combPropType.ItemsSource = Enum.GetValues(typeof(PropType));

            txtProjName.DataContext = CurProject;
            txtProDesc.DataContext = CurProject;
            txtFileNaming.DataContext = CurProject;
            txtSceneNaming.DataContext = CurProject;
            txtCharacterNaming.DataContext = CurProject;
            txtPropNaming.DataContext = CurProject;
            txtCamNaming.DataContext = CurProject;

            if (!string.IsNullOrEmpty(CurProject.Name))
            {
                txtProjName.IsEnabled = false;
            }
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

            Property property = CurProject.AddProperty(propName);
            if (null == property)
            {
                MessageBox.Show("属性已经存在!", "Error");
                return;
            }

            property.Name = propName;
            property.Type = propType;
            property.Value = propVal;
            property.Desc = txtDesc.Text;
            property.ReadOnly = false;
            //刷新列表
            propGrid.ItemsSource = CurProject.PropertySet;
        }

        private void ButDelProp_Click(object sender, RoutedEventArgs e)
        {
            Property property = CurProject.GetProperty(txtPropName.Text);
            if (null == property)
            {
                MessageBox.Show("没有此属性!", "Error");
                return;
            }
            
            if (property.ReadOnly)
            {
                MessageBox.Show("当前属性不可被删除!", "Error");
                return;
            }

            CurProject.RemoveProperty(txtPropName.Text);

            //刷新列表
            propGrid.ItemsSource = CurProject.PropertySet;
        }

        private void ButModifyProp_Click(object sender, RoutedEventArgs e)
        {
            Property property = CurProject.GetProperty(txtPropName.Text);
            if (null == property)
            {
                MessageBox.Show("没有此属性!", "Error");
                return;
            }

            if (property.ReadOnly)
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
            propGrid.ItemsSource = CurProject.PropertySet;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string propjName = txtProjName.Text;
            string propDesc = txtProDesc.Text;
            string fileNaming = txtFileNaming.Text;
            string sceneNaming = txtSceneNaming.Text;
            string charNaming = txtCharacterNaming.Text;
            string propNaming = txtPropNaming.Text;
            string camNaming = txtCamNaming.Text;

            if (string.IsNullOrEmpty(propjName))
            {
                txtProjName.SetValue(StyleProperty, FindResource("errTextBox"));
                MessageBox.Show("输入项目名称", "Error");
                return;
            }
            else
            {
                txtProjName.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查文件命名合法性
            if (!string.IsNullOrEmpty(fileNaming))
            {
                string[] result = fileNaming.Split('_');
                foreach(var prop in result)
                {
                    if (!CurProject.FindProperty(prop))
                    {
                        MessageBox.Show("属性不存在:" + prop, "Error");
                        txtFileNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtFileNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查场景命名合法性
            if (!string.IsNullOrEmpty(sceneNaming))
            {
                string[] result = sceneNaming.Split('_');
                foreach (var prop in result)
                {
                    if (!CurProject.FindProperty(prop))
                    {
                        MessageBox.Show("属性不存在:" + prop, "Error");
                        txtSceneNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtSceneNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查角色命名合法性
            if (!string.IsNullOrEmpty(charNaming))
            {
                string[] result = charNaming.Split('_');
                foreach (var prop in result)
                {
                    if (!CurProject.FindProperty(prop))
                    {
                        MessageBox.Show("属性不存在:" + prop, "Error");
                        txtCharacterNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtCharacterNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查道具命名合法性
            if (!string.IsNullOrEmpty(propNaming))
            {
                string[] result = propNaming.Split('_');
                foreach (var prop in result)
                {
                    if (!CurProject.FindProperty(prop))
                    {
                        MessageBox.Show("属性不存在:" + prop, "Error");
                        txtPropNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtPropNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            //检查相机命名合法性
            if (!string.IsNullOrEmpty(camNaming))
            {
                string[] result = camNaming.Split('_');
                foreach (var prop in result)
                {
                    if (!CurProject.FindProperty(prop))
                    {
                        MessageBox.Show("属性不存在:" + prop, "Error");
                        txtCamNaming.SetValue(StyleProperty, FindResource("errTextBox"));
                        return;
                    }
                }
                txtCamNaming.SetValue(StyleProperty, FindResource("norTextBox"));
            }

            // 创建模式
            if (string.IsNullOrEmpty(CurProject.Name))
            {
                Project project = (Application.Current as App).GetProject(propjName);
                if (null != project)
                {
                    txtProjName.SetValue(StyleProperty, FindResource("errTextBox"));
                    MessageBox.Show("已经存在相同的项目名", "Error");
                    return;
                }
            }

            CurProject.Name = propjName;
            CurProject.Description = propDesc;
            CurProject.FileNaming = fileNaming;
            CurProject.CharNaming = charNaming;
            CurProject.PropNaming = propNaming;
            CurProject.SceneNaming = sceneNaming;
            CurProject.CamNaming = camNaming;

            (Application.Current as App).AddProject(CurProject);

            Close();
        }
    }
}
