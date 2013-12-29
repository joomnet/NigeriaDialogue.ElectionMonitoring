using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        public IEnumerable<Project> GetProjects()
        {
            return entities.Projects.ToArray();
        }

        public Project GetProject(int projectId)
        {
            return entities.Projects.FirstOrDefault(p => p.ProjectID == projectId);
        }

        public int CreateProject(Project project)
        {
            entities.Projects.Add(project);
            int newID = entities.SaveChanges();
            return newID;
        }

        public bool UpdateProject(Project project)
        {
            Project dataProject = entities.Projects.SingleOrDefault(p => p.ProjectID == project.ProjectID);
            if (dataProject != null)
            {
                dataProject.Title = project.Title;
                dataProject.Description = project.Description;
                dataProject.Budget = project.Budget;

                int updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteProject(int projectID)
        {
            Project dataProject = entities.Projects.SingleOrDefault(p => p.ProjectID == projectID);
            if (dataProject != null)
            {
                entities.Projects.Remove(dataProject);
                return true;
            }
            return false;
        }
    }
}