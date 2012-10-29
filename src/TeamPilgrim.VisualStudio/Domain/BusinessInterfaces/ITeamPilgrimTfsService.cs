using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimTfsService
    {
        TfsTeamProjectCollection[] GetProjectCollections();
        TfsTeamProjectCollection GetProjectCollection(Uri uri);

        Project[] GetProjects(Uri tpcAddress);
        ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress);
        RegisteredProjectCollection[] GetRegisteredProjectCollections();
        
        bool DeleteQueryDefintion(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryId);
    }
}