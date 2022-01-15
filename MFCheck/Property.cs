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
        public string Name { set; get; }

        // 属性类型
        public PropType Type { set; get; }

        // 属性描述
        public string Desc { set; get; }

        // 属性值
        public string Value { set; get; }

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
            string[] enumList = Value.Split(',');
            foreach(var e in enumList)
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
    }
}
