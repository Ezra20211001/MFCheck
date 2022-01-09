using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFCheck
{
    class Property
    {
        // 属性名
        public string Name { set; get; }

        // 属性类型
        public PropType Type { set; get; }

        // 属性描述
        public string Desc { set; get; }
    }
}
