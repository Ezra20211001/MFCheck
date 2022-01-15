using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFCheck
{
    public class Project
    {
        public Project()
        {
            //创建默认属性
            Property property = AddProperty("Project");
            property.Name = "Project";
            property.Type = PropType.Const;
            property.ReadOnly = true;
            property.Value = "";
            property.Desc = "项目名称，项目创建后自动绑定项目名称";
        }

        // 项目名称
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                MainProperty.Value = value;
            }
        }

        // 项目描述
        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set
            {
                m_Description = value;
                MainProperty.Desc = value;
            }
        }

        // 模型命名方式
        public string ModNaming { get; set; }

        // 相机命名方式
        public string CamNaming { get; set; }

        // 文件命名
        public string FileNaming { get; set; }

        // 主属性
        public Property MainProperty { get => m_Properties["Project"]; }

        // 属性列表
        public List<Property> PropertySet { get => m_Properties.Values.ToList(); }

        public Property GetProperty(string name)
        {
            if (m_Properties.ContainsKey(name))
            {
                return m_Properties[name];
            }

            return null;
        }

        public bool FindProperty(string name)
        {
            if (m_Properties.ContainsKey(name))
            {
                return true;
            }

            return false;
        }

        public Property AddProperty(string name)
        {
            if (m_Properties.ContainsKey(name))
            {
                return null;
            }

            Property property = new Property();
            m_Properties.Add(name, property);

            return property;
        }

        public void RemoveProperty(string name)
        {
            if (m_Properties.ContainsKey(name))
            {
                m_Properties.Remove(name);
            }
        }

        private Dictionary<string, Property> m_Properties = new Dictionary<string, Property>();
    }
}
