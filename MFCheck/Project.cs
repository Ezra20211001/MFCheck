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
            property.ReadOnley = true;
            property.Value = "";
            property.Desc = "项目名称，项目创建后自动绑定项目名称";
        }

        // 项目名称
        public string Name { get; set; }

        // 项目描述
        public string Description { get; set; }

        // 模型命名方式
        public string ModNaming { get; set; } = "AAA_AAA_AAAA_AAA";

        // 相机命名方式
        public string CamNaming { get; set; } = "AAA_AAA_AAAA_AAA";

        // 文件命名
        public string FileNaming { get; set; } = "";

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
