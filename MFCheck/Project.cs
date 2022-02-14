using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public string Description
        {
            get { return m_Description; }
            set
            {
                m_Description = value;
                MainProperty.Desc = value;
            }
        }

        // 文件命名
        public string FileNaming
        {
            get { return m_FileNaming; }
            set
            {
                m_FileNaming = value;
                if (!string.IsNullOrEmpty(m_FileNaming))
                {
                    m_FileProps = m_FileNaming.Split(SPILT_CHA);
                }
                else
                {
                    m_FileProps = null;
                }
            }
        }

        // 模型命名方式
        public string ModNaming
        {
            get { return m_ModNaming; }
            set
            {
                m_ModNaming = value;
                if (!string.IsNullOrEmpty(m_ModNaming))
                {
                    m_ModProps = m_ModNaming.Split(SPILT_CHA);

                    foreach (var prop in m_ModProps)
                    {
                        m_ModRegex += GetProperty(prop).Value + SPILT_CHA;
                    }
                }
                else
                {
                    m_ModProps = null;
                    m_ModRegex = "";
                }
            }
        }

        // 相机命名方式
        public string CamNaming
        {
            get { return m_CamNaming; }
            set
            {
                m_CamNaming = value;
                if (!string.IsNullOrEmpty(m_CamNaming))
                {
                    m_CamProps = m_CamNaming.Split(SPILT_CHA);
                    foreach (var prop in m_CamProps)
                    {
                        m_CamRegex += GetProperty(prop).Value + SPILT_CHA;
                    }
                }
                else
                {
                    m_CamProps = null;
                    m_CamRegex = "";
                }
            }
        }

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
            return null != GetProperty(name);
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

        // 测试文件命名是否正确
        public bool TestingFileName(string name)
        {
            if (string.IsNullOrEmpty(FileNaming))
            {
                return false;
            }

            string[] nameSplit = name.Split(SPILT_CHA);
            if (m_FileProps.Count() != nameSplit.Count())
            {
                return false;
            }

            for (int i = 0; i < m_FileProps.Count(); ++i)
            {
                if (!GetProperty(m_FileProps[i]).Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // 是否是模型名称
        public bool IsModName(string name)
        {
            return Regex.IsMatch(name, m_ModRegex);
        }

        // 测试模型名称是否正确
        public bool TestingModName(string name)
        {
            if (string.IsNullOrEmpty(ModNaming))
            {
                return true;
            }

            string[] nameSplit = name.Split(SPILT_CHA);
            if (nameSplit.Count() != m_ModProps.Count())
            {
                return false;
            }

            for (int i = 0; i < m_ModProps.Count(); ++i)
            {
                if (!GetProperty(m_FileProps[i]).Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // 是否是相机名称
        public bool IsCameraName(string name)
        {
            return Regex.IsMatch(name, m_CamRegex);
        }

        // 测试相机名称是否正确
        public bool TestingCamName(string name)
        {
            if (string.IsNullOrEmpty(m_CamNaming))
            {
                return true;
            }

            string[] nameSplit = name.Split(SPILT_CHA);
            if (nameSplit.Count() != m_CamProps.Count())
            {
                return false;
            }

            for (int i = 0; i < m_CamProps.Count(); ++i)
            {
                if (!GetProperty(m_CamProps[i]).Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }


        private const char SPILT_CHA = '_';
        private string m_Name;
        private string m_Description;
        private string m_FileNaming;
        private string m_ModNaming;
        private string m_CamNaming;
        private string[] m_FileProps;
        private string[] m_ModProps;
        private string[] m_CamProps;
        private string m_ModRegex = "";
        private string m_CamRegex = "";
        private Dictionary<string, Property> m_Properties = new Dictionary<string, Property>();
    }
}
