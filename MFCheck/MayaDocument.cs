using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFCheck
{
    class MayaDocument
    {
        // 错误信息
        public string Error { get; private set; }

        // 全路径
        public string FullName { get; private set; }

        // 编辑版本
        public string Product { get; private set; }

        // 源文件名
        public string OriginName { get; private set; }

        // 最后修改时间
        public string LastModify { get; private set; }

        // Root元素列表
        public List<MayaElement> RootElements { get; } = new List<MayaElement>();

        // 获得所有引用信息
        public List<MayaReference> MayaReferenceList { get => m_References.Values.ToList(); }

        // 引用信息
        public MayaReference FindReference(string name)
        {
            if (m_References.ContainsKey(name))
            {
                return m_References[name];
            }
            return null;
        }

        //获得元素信息
        public MayaElement GetElement(string name)
        {
            if (m_NameIndices.ContainsKey(name))
            {
                return m_NameIndices[name];
            }

            return null;
        }

        public bool Load(string fullName)
        {
            FullName = fullName;

            StreamReader reader = new StreamReader(fullName);
            if (reader == null)
            {
                Error = "无法打开文件: " + fullName;
                return false;
            }

 //           try
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string sub = line.Substring(0, 2);
                    if (sub == "//")
                    {
                        OnExplainCommand(line, reader);
                        continue;
                    }

                    string[] subs = line.Split(' ');
                    if (null == subs || subs.Count() < 1)
                    {
                        //无法识别的命令
                        continue;
                    }

                    string cmd = subs[0];
                    if (cmd == "file")
                    {
                        OnFileCommand(line, reader);
                    }
                    else if (cmd == "fileInfo")
                    {
                        OnFileInfoCommand(line, reader);
                    }
                    else if (cmd == "requires")
                    {
                        OnRequeiresCommand(line, reader);
                    }
                    else if (cmd == "createNode")
                    {
                        OnCreateCommand(line, reader);
                    }
                    else
                    {

                    }
                }

                return OnReadComplete();
            }
//            catch (Exception ex)
            {
//                Error = ex.ToString();
 //               return false;
            }
 //           finally
            {
                reader.Close();
            }
        }

        //注释
        private bool OnExplainCommand(string command, StreamReader reader)
        {
            string nameKey = "//Name:";
            string tiemKey = "//Last modified:";
            if (command.Contains(nameKey))
            {
                string fileName = command.Substring(nameKey.Count());
                OriginName = fileName.Trim();
            }
            else if (command.Contains(tiemKey))
            {
                string modifyTime = command.Substring(tiemKey.Count());
                LastModify = modifyTime.Trim();
            }

            return true;
        }

        private bool OnFileCommand(string command, StreamReader reader)
        {
            MayaReference reference = new MayaReference(this);
            reference.ParseCommand(command, reader);

            if (!m_References.ContainsKey(reference.ReferenceNode))
            {
                m_References.Add(reference.ReferenceNode, reference);
            }

            return true;
        }

        //fileInfo 命令
        private bool OnFileInfoCommand(string command, StreamReader reader)
        {
            return true;
        }

        //requires命令
        private bool OnRequeiresCommand(string command, StreamReader reader)
        {
            return true;
        }

        //create命令
        private bool OnCreateCommand(string command, StreamReader reader)
        {
            MayaElement node = new MayaElement(this);
            if (!node.ParseCommand(command, reader))
            {
                throw new Exception("无法解析命令！");
            }

            m_NameIndices.Add(node.Name, node);

            //父节点为空，则判定为Root节点
            if (string.IsNullOrEmpty(node.ParentName))
            {
                RootElements.Add(node);
            }
            else
            {
                //添加节点树信息
                if (m_TreeInfo.ContainsKey(node.ParentName))
                {
                    m_TreeInfo[node.ParentName].Add(node.Name);
                }
                else
                {
                    List<string> children = new List<string>
                    {
                        node.Name
                    };
                    m_TreeInfo.Add(node.ParentName, children);
                }
            }


            return true;
        }

        //读取结束，调整树结构
        private bool OnReadComplete()
        {
            foreach (var node in m_TreeInfo)
            {
                MayaElement parent = m_NameIndices[node.Key];
                foreach (var childName in node.Value)
                {
                    parent.AddChild(m_NameIndices[childName]);
                }
            }

            m_TreeInfo.Clear();
            return true;
        }

        private Dictionary<string, MayaReference> m_References = new Dictionary<string, MayaReference>();
        private Dictionary<string, MayaElement> m_NameIndices = new Dictionary<string, MayaElement>();
        private Dictionary<string, List<string>> m_TreeInfo = new Dictionary<string, List<string>>();
    }

    class MayaElementType
    {
        public static readonly string MERefernce = "reference";
        public static readonly string MEScript = "script";
        public static readonly string METransform = "transform";
        public static readonly string MECamera = "camera";
    }

    class MayaElement
    {
        public MayaElement(MayaDocument document)
        {
            Document = document;
        }

        private readonly List<MayaElement> m_Children = new List<MayaElement>();

        //类型
        public string Type { get; private set; }

        //节点名称
        public string Name { get; private set; }

        //父节点名称
        public string ParentName { get; private set; }

        //创建命令
        public string Command { get; private set; }

        //节点属性
        public string Attribute { get; private set; } = "";

        //重命名命令
        public string Rename { get; private set; }

        //Maya文档
        public MayaDocument Document { get; private set; }

        public List<MayaElement> Children { get => m_Children.ToList(); }

        //解析命令
        public bool ParseCommand(string command, StreamReader reader)
        {
            List<string> createSession = new List<string>();
            string tempCmd = "";

            //查看当前命令是否完整
            if (';' == command.Last())
            {
                createSession.Add(command.Substring(0, command.Count() - 1));
            }
            else
            {
                tempCmd = command;
            }

            while (reader.Peek() == '\t')
            {
                string line = reader.ReadLine();
                while (line.ElementAt(0) == '\t')
                {
                    line = line.Remove(0, 1);
                }

                if (';' == line.Last())
                {
                    tempCmd += line;
                    createSession.Add(tempCmd.Substring(0, tempCmd.Count() - 1));
                    tempCmd = "";
                }
                else
                {
                    tempCmd += line;
                }
            }

            Command = createSession.ElementAt(0).Replace("\"", "");

            string[] subs = Command.Split(' ');
            for (int i = 0; i < subs.Count(); ++i)
            {
                string param = subs[i];
                if (param == "createNode")
                {
                    Type = subs[++i];
                }
                else if (param == "-name" || param == "-n")
                {
                    Name = subs[++i];
                    if (string.IsNullOrEmpty(Name))
                        return false;
                }
                else if (param == "-parent" || param == "-p")
                {
                    ParentName = subs[++i];
                }
            }

            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Name))
            {
                return false;
            }

            return true;
        }

        //添加子元素
        public bool AddChild(MayaElement child)
        {
            m_Children.Add(child);
            return true;
        }
    }

    //Maya引用
    class MayaReference
    {
        public MayaReference(MayaDocument document)
        {
            RootDocument = document;
        }

        //引用命令
        public string Command { get; private set; }

        //引用文件
        public string ReferenceFile { get; private set; }

        //引用文件类型
        public string RefrenceFileType { get; private set; }

        //引用节点(createNode "ReferenceNode")
        public string ReferenceNode { get; private set; }

        // 命名空间
        public string NameSpace { get; private set; }

        // 根文件
        public MayaDocument RootDocument { get; private set; }

        // 根文件
        public MayaDocument Document { get; set; }

        //解析命令
        public bool ParseCommand(string command, StreamReader reader)
        {
            //全部读取
            Command = command;
            while (reader.Peek() == '\t')
            {
                string temp = reader.ReadLine();

                //移除前面的制表符
                while(temp.ElementAt(0) == '\t')
                {
                    temp = temp.Remove(0, 1);
                }

                Command += temp;
                if (Command.ElementAt(Command.Count() - 1) == ';')
                {
                    Command = Command.Substring(0, Command.Count() - 1);
                    break;
                }
            }

            int index = 0;
            int count = Command.Count();
            string option = "";
            while (index < count)
            {
                // 读取操作符
                option = "";
                char c = Command.ElementAt(index++);
                if (c == '-')
                {
                    while (c != ' ')
                    {
                        option += c;
                        c = Command.ElementAt(index++);
                    }
                }

                // 读取参数
                if (option == "-ns" || option == "-namespace")
                {
                    int start = 0;
                    int length = -1;
                    while (true)
                    {
                        c = Command.ElementAt(index++);
                        if (c == '\"')
                        {
                            if (start == 0)
                            {
                                start = index;
                            }
                            else
                            {
                                break;
                            }
                        }
                        length++;
                    }

                    NameSpace = Command.Substring(start, length);
                }
                else if (option == "-rfn" || option == "-referenceNode")
                {
                    int start = 0;
                    int length = -1;
                    while (true)
                    {
                        c = Command.ElementAt(index++);
                        if (c == '\"')
                        {
                            if (start == 0)
                            {
                                start = index;
                            }
                            else
                            {
                                break;
                            }
                        }
                        length++;
                    }
                    ReferenceNode = Command.Substring(start, length);
                }
                else if (option == "-typ" || option == "-type")
                {
                    int start = 0;
                    int length = -1;
                    while (true)
                    {
                        c = Command.ElementAt(index++);
                        if (c == '\"')
                        {
                            if (start == 0)
                            {
                                start = index;
                            }
                            else
                            {
                                break;
                            }
                        }
                        length++;
                    }

                    RefrenceFileType = Command.Substring(start, length);

                    start = 0;
                    length = -1;
                    while (true)
                    {
                        c = Command.ElementAt(index++);
                        if (c == '\"')
                        {
                            if (start == 0)
                            {
                                start = index;
                                length = -1;
                            }
                            else
                            {
                                break;
                            }
                        }
                        length++;
                    }

                    ReferenceFile = Command.Substring(start, length);
                }
            }

            return true;
        }
    }
}
