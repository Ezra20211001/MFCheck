using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                if (string.IsNullOrEmpty(m_FileNaming))
                {
                    m_FileProps = null;
                }
                else
                {
                    m_FileProps = m_FileNaming.Split(SPILT_CHA);
                }
            }
        }

        // 检查文件命名是否正确
        public bool TestFileName(string name)
        {
            if (string.IsNullOrEmpty(m_FileNaming))
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

        // 场景命名
        public string SceneNaming
        {
            get { return m_SceneNaming; }
            set
            {
                m_SceneNaming = value;
                if (string.IsNullOrEmpty(m_SceneNaming))
                {
                    m_SceneProps = null;
                }
                else
                {
                    m_SceneProps = m_SceneNaming.Split(SPILT_CHA);
                }
            }
        }

        // 检查场景命名是否正确
        public bool TestSceneName(string name)
        {
            if (string.IsNullOrEmpty(m_SceneNaming))
            {
                return true;
            }

            string[] nameSplit = name.Split(SPILT_CHA);
            if (nameSplit.Count() != m_SceneProps.Count())
            {
                return false;
            }

            for (int i = 0; i < m_SceneProps.Count(); ++i)
            {
                if (!GetProperty(m_SceneProps[i]).Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // 角色命名
        public string CharNaming
        {
            get { return m_CharNaming; }
            set
            {
                m_CharNaming = value;
                if (string.IsNullOrEmpty(m_CharNaming))
                {
                    m_CharProps = null;
                }
                else
                {
                    m_CharProps = m_CharNaming.Split(SPILT_CHA);
                }
            }
        }

        // 检查角色命名是否正确
        public bool TestCharName(string name)
        {
            if (string.IsNullOrEmpty(m_CharNaming))
            {
                return true;
            }

            string[] nameSplit = name.Split(SPILT_CHA);
            if (nameSplit.Count() != m_CharProps.Count())
            {
                return false;
            }

            for (int i = 0; i < m_CharProps.Count(); ++i)
            {
                if (!GetProperty(m_CharProps[i]).Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // 道具命名
        public string PropNaming
        {
            get { return m_PropNaming; }
            set
            {
                m_PropNaming = value;
                if (string.IsNullOrEmpty(m_PropNaming))
                {
                    m_PropProps = null;
                }
                else
                {
                    m_PropProps = m_PropNaming.Split(SPILT_CHA);
                }
            }
        }

        // 检查道具命名是否正确
        public bool TestPropName(string name)
        {
            if (string.IsNullOrEmpty(m_PropNaming))
            {
                return true;
            }

            string[] nameSplit = name.Split(SPILT_CHA);
            if (nameSplit.Count() != m_PropProps.Count())
            {
                return false;
            }

            for (int i = 0; i < m_PropProps.Count(); ++i)
            {
                if (!GetProperty(m_PropProps[i]).Testing(nameSplit[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // 相机命名方式
        public string CamNaming
        {
            get { return m_CamNaming; }
            set
            {
                m_CamNaming = value;
                if (string.IsNullOrEmpty(m_CamNaming))
                {
                    m_CamProps = null;
                }
                else
                {
                    m_CamProps = m_CamNaming.Split(SPILT_CHA);
                }
            }
        }

        // 测试相机名称是否正确
        public bool TestCamName(string name)
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

        // 是否是模型名称
        public bool IsModName(string name)
        {
            return name.StartsWith(Name);
        }

        private const char SPILT_CHA = '_';
        private string m_Name;
        private string m_Description;

        //文件命名
        private string m_FileNaming;
        private string[] m_FileProps;

        //场景命名
        private string m_SceneNaming;
        private string[] m_SceneProps;

        //角色命名
        private string m_CharNaming;
        private string[] m_CharProps;

        //道具命名
        private string m_PropNaming;
        private string[] m_PropProps;

        //相机命名
        private string m_CamNaming;
        private string[] m_CamProps;
        
        private Dictionary<string, Property> m_Properties = new Dictionary<string, Property>();
    }
}
