using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFCheck
{
    public class ProjectSet
    {
        public Project GetProject(string name)
        {
            if (m_Projects.ContainsKey(name))
            {
                return m_Projects[name];
            }
            return null;
        }

        public List<Project> GetProjectList()
        {
            return m_Projects.Values.ToList();
        }

        public bool AddProject(Project project)
        {
            if (m_Projects.ContainsKey(project.Name))
            {
                return false;
            }

            m_Projects.Add(project.Name, project);
            return true;
        }

        public void RemoveProject(string name)
        {
            if (m_Projects.ContainsKey(name))
            {
                m_Projects.Remove(name);
            }
        }

        private Dictionary<string, Project> m_Projects = new Dictionary<string, Project>();
    }
}
