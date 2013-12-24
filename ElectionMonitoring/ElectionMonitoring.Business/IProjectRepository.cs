using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    public interface IProjectRepository
    {
        IEnumerable<Models.Project> GetProjects();
        Models.Project GetProject(int ProjectID);
        int CreateProject(Models.Project project);
        bool UpdateProject(Models.Project project);
        bool DeleteProject(int ProjectID);
    }
}
