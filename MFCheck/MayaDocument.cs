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

        // 文件名
        public string Name { get; private set; }

        // 全路径
        public string FullName { get; private set; }

        // 编辑版本
        public string Product { get; private set; }



        public List<string> RootElements
        {
            get => m_RootElement.Keys.ToList();
        }

        public List<string> ChildElemetnt(string name)
        {
            if (m_NodeElement.ContainsKey(name))
            {
                return m_NodeElement.Keys.ToList();
            }

            return null;
        }

        public MayaElement GetMayaElement(string name)
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
            Name = Path.GetFileName(fullName);

            StreamReader reader = new StreamReader(fullName);
            if (reader == null)
            {
                Error = "无法打开文件: " + fullName;
                return false;
            }

            try
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

                    string[] subs = line.Split(new char[] { ' ' });
                    if (null == subs || subs.Count() < 1)
                    {
                        //无法识别的命令
                        continue;
                    }

                    string cmd = subs[0];
                    if (cmd == "fileInfo")
                    {
                        OnFileCommand(line, reader);
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

                return true;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                return false;
            }
            finally
            {
                reader.Close();
            }
        }

        //注释
        private bool OnExplainCommand(string command, StreamReader reader)
        {
            return true;
        }

        //file 命令
        private bool OnFileCommand(string command,  StreamReader reader)
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
            MayaElement node = new MayaElement();
            if (!node.ParseCommand(command, reader))
            {
                throw new Exception("无法解析命令！");
            }

            m_NameIndices.Add(node.NodeName, node);

            //父节点为空，则判定为Root节点
            if (string.IsNullOrEmpty(node.ParentName))
            {
                List<string> children = new List<string>();
                children.Add(node.NodeName);

                m_RootElement.Add(node.ParentName, children);
            }
            else
            {
                if (!m_NodeElement.ContainsKey(node.ParentName))
                {
                    m_NodeElement[node.ParentName].Add(node.NodeName);
                }
                else
                {
                    List<string> children = new List<string>();
                    children.Add(node.NodeName);

                    m_NodeElement.Add(node.ParentName, children);
                }
            }

            return true;
        }

        //读取结束，调整树结构
        private bool OnReadComplete()
        {
            return true;
        }

        private Dictionary<string, MayaElement> m_NameIndices = new Dictionary<string, MayaElement>();
        private Dictionary<string, List<string>> m_NodeElement = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> m_RootElement = new Dictionary<string, List<string>>();
    }


    class MayaElement
    {
        //类型
        public string Type { get; private set; }

        //节点名称
        public string NodeName { get; private set; }

        //父节点名称
        public string ParentName { get; private set; }

        //创建命令
        public string Command { get; private set; }

        //解析命令
        public bool ParseCommand(string command, StreamReader reader)
        {
            Command = command;

            string pureCmd = command.Replace(";", "");
            pureCmd = pureCmd.Replace("\"", "");

            string[] subs = pureCmd.Split(new char[] { ' ' });
            for (int i = 0; i < subs.Count(); ++i)
            {
                string param = subs[i];
                if (param == "createNode")
                {
                    Type = subs[++i];
                }
                else if (param == "-name" || param == "-n")
                {
                    NodeName = subs[++i];
                    if (string.IsNullOrEmpty(NodeName))
                        return false;
                }
                else if (param == "-parent" || param == "-p")
                {
                    ParentName = subs[++i];
                }
            }

            //查看下一行内容
            int tab = '\t';
            int peek;
           
            while ((peek = reader.Peek()) == tab)
            {
                string nodeAttr = reader.ReadLine();
            }

            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(NodeName))
            {
                return false;
            }

            return true;
        }
    }
}
