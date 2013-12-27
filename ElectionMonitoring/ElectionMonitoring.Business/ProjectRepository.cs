using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ElectionMonitoring.Business
{
    using ElectionMonitoring.Models;
    using AutoMapper;
    public class ProjectRepository : IProjectRepository
    {
        private Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();

        public IEnumerable<Models.Project> GetProjects()
        {
            var dataProjects = entities.Projects;
            Mapper.CreateMap<Data.Project, Models.Project>();
            var modelProjects = Mapper.Map<IEnumerable<Data.Project>, IEnumerable<Models.Project>>(dataProjects);
            return modelProjects;
        }

        public Models.Project GetProject(int ProjectID)
        {
            var dataProject = entities.Projects.SingleOrDefault(p => p.ProjectID == ProjectID);
            Mapper.CreateMap<Data.Project, Models.Project>();
            var modelProject = Mapper.Map<Data.Project, Models.Project>(dataProject);
            return modelProject;
        }

        public int CreateProject(Models.Project project)
        {
            Mapper.CreateMap<Models.Project, Data.Project>();
            var dataProject = Mapper.Map<Models.Project, Data.Project>(project);
            entities.AddToProjects(dataProject);
            var newID = entities.SaveChanges();
            return newID;
        }

        public bool UpdateProject(Models.Project project)
        {
            var dataProject = entities.Projects.SingleOrDefault(p => p.ProjectID == project.ProjectID);
            if (dataProject != null)
            {
                dataProject.Title = project.Title;
                dataProject.Description = project.Description;
                dataProject.Budget = project.Budget;

                var updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteProject(int projectID)
        {
            var dataProject = entities.Projects.SingleOrDefault(p => p.ProjectID == projectID);
            if (dataProject != null)
            {
                entities.Projects.DeleteObject(dataProject);
                return true;
            }
            return false;
        }
    }
}
