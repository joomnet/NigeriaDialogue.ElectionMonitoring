using System.Collections.Generic;

namespace ElectionMonitoring.Repository
{
    public interface IProjectRepository
    {
        IEnumerable<Models.Project> GetProjects();
        Models.Project GetProject(int projectId);
        int CreateProject(Models.Project project);
        bool UpdateProject(Models.Project project);
        bool DeleteProject(int ProjectID);
    }
}
