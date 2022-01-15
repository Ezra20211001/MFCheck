using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MFCheck
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) 
        {
            base.OnStartup(e);

            //加载配置
            string fullPath = ConfigPath + ConfigName;

            try
            {
                XDocument xmlDocument = XDocument.Load(fullPath);
                foreach (var proj in xmlDocument.Descendants("Project"))
                {
                    if (null != Manager.GetProject(proj.Attribute("Name").Value))
                    {
                        continue;
                    }

                    Project project = new Project()
                    {
                        Name = proj.Attribute("Name").Value,
                        Description = proj.Attribute("Description").Value,
                        FileNaming = proj.Attribute("FileNaming").Value,
                        ModNaming = proj.Attribute("ModNaming").Value,
                        CamNaming = proj.Attribute("CamNaming").Value,
                    };

                    project.MainProperty.Value = project.Name;
                    project.MainProperty.Desc = project.Description;

                    foreach (var prop in proj.Descendants("Property"))
                    {
                        if (project.FindProperty(prop.Attribute("Name").Value))
                        {
                            continue;
                        }

                        Property property = project.AddProperty(prop.Attribute("Name").Value);
                        property.Name = prop.Attribute("Name").Value;
                        property.Type = (PropType)Enum.Parse(typeof(PropType), prop.Attribute("Type").Value);
                        property.Desc = prop.Attribute("Desc").Value;
                        property.Value = prop.Attribute("Value").Value;
                        property.ReadOnly = bool.Parse(prop.Attribute("ReadOnly").Value);
                    }

                    Manager.AddProject(project);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
                Current.Shutdown(-1);
            }

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            string fullPath = ConfigPath + ConfigName;

            //导出配置
            XDocument xmlDocument = new XDocument();
            xmlDocument.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            
            XElement root = new XElement("ProjectSet");
            xmlDocument.Add(root);

            List<Project> ProjectList = Manager.GetProjectList();
            foreach(var project  in ProjectList)
            {
                XElement projEle = new XElement("Project");
                root.Add(projEle);

                projEle.Add(new XAttribute("Name", project.Name));
                projEle.Add(new XAttribute("Description", project.Description));
                projEle.Add(new XAttribute("FileNaming", project.FileNaming));
                projEle.Add(new XAttribute("ModNaming", project.ModNaming));
                projEle.Add(new XAttribute("CamNaming", project.CamNaming));

                foreach (var property in project.PropertySet)
                {
                    if (property == project.MainProperty)
                    {
                        continue;
                    }

                    XElement propEle = new XElement("Property");
                    projEle.Add(propEle);

                    propEle.Add(new XAttribute("Name", property.Name));
                    propEle.Add(new XAttribute("Type", property.Type.ToString()));
                    propEle.Add(new XAttribute("Desc", property.Desc));
                    propEle.Add(new XAttribute("Value", property.Value));
                    propEle.Add(new XAttribute("ReadOnly", property.ReadOnly.ToString()));
                }
            }

            xmlDocument.Save(fullPath);
        }

        public ProjectSet Manager { get; private set; } = new ProjectSet();
        public string ConfigName { get; private set; } = "config.xml";
        public string ConfigPath { get; private set; } = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
    }
}
