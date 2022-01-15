using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFCheck
{
    public class Property
    {
        // 属性名
        public string Name { get; set; }

        // 属性类型
        public PropType Type { get; set; }

        // 属性描述
        public string Desc { get; set; }

        // 属性值
        public string Value
        {
            get { return m_StrValue; }
            set
            {
                m_StrValue = value;
                if (string.IsNullOrEmpty(m_StrValue))
                {
                    m_Contents = null;
                }
                else
                {
                    m_Contents = m_StrValue.Split(SPILT_CHA);
                }
            }
        }

        // 获取值列表
        //public string[] Values { get => m_Contents; }

        // 只读属性
        public bool ReadOnly { get; set; }

        // 验证
        public bool Testing(string value)
        {
            switch (Type)
            {
                case PropType.Const:
                    return TestingConst(value);
                case PropType.Enum:
                    return TestingEnum(value);
                case PropType.Variable:
                    return TestingVariable(value);
                default:
                    return false;
            }
        }

        private bool TestingConst(string value)
        {
            return Value == value;
        }

        private bool TestingEnum(string value)
        {
            foreach(var e in m_Contents)
            {
                if (e == value)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TestingVariable(string value)
        {
            return true;
        }

        private const char SPILT_CHA = ',';
        private string m_StrValue;
        private string[] m_Contents;
    }
}
