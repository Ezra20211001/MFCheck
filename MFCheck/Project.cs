using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFCheck
{
    class Project
    {
        // 项目名称
        public string Name { get; set; }

        // 获得属性列表
        public int GetCount()
        {
            return m_Properties.Count;
        }

        public Property GetProperty(string name)
        {
            return m_Properties[name];
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
            Property property = new Property();
            property.Name = name;

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
