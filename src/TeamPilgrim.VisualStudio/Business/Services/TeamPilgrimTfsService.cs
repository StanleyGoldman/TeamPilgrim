using System;
using System.Collections.Concurrent;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class TeamPilgrimTfsService : ITeamPilgrimTfsService
    {
        private readonly ConcurrentDictionary<string, TfsTeamProjectCollection> _projectCollectionDictionary = new ConcurrentDictionary<string, TfsTeamProjectCollection>();

        public TfsTeamProjectCollection GetProjectCollection(Uri uri)
        {
            if (uri == null)
                return null;

            return _projectCollectionDictionary.GetOrAdd(uri.ToString(), s =>
                {
                    var tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri);
                    tfsTeamProjectCollection.Authenticate();

                    return tfsTeamProjectCollection;
                });
        }

        public RegisteredProjectCollection[] GetRegisteredProjectCollections()
        {
            return RegisteredTfsConnections.GetProjectCollections();
        }

        public bool DeleteQueryDefintion(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryId)
        {
            var workItemStore = GetWorkItemStore(teamProjectCollection);
            var queryHierarchy = workItemStore.GetQueryHierarchy(teamProject);
            var queryItem = queryHierarchy.Find(queryId);

            if (queryItem == null)
                return false;

            queryItem.Delete();
            queryHierarchy.Save();
            return true;
        }

        public TfsTeamProjectCollection[] GetProjectCollections()
        {
            RegisteredProjectCollection[] registeredProjectCollections = GetRegisteredProjectCollections();

            return registeredProjectCollections.Select(collection => new TfsTeamProjectCollection(collection)).ToArray();
        }

        public Project[] GetProjects(Uri tpcAddress)
        {
            return GetProjects(GetProjectCollection(tpcAddress));
        }

        private Project[] GetProjects(TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            var workItemStore = new WorkItemStore(tfsTeamProjectCollection);

            return workItemStore.Projects.Cast<Project>().ToArray();
        }

        public ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress)
        {
            var collection = GetProjectCollection(tpcAddress);
            return GetTeamPilgrimBuildService(collection);
        }

        private ITeamPilgrimBuildService GetTeamPilgrimBuildService(TfsTeamProjectCollection collection)
        {
            var buildServer = collection.GetService<IBuildServer>();

            return new TeamPilgrimBuildService(buildServer);
        }

        private WorkItemStore GetWorkItemStore(Uri tpcAddress)
        {
            return GetWorkItemStore(GetProjectCollection(tpcAddress));
        }

        private WorkItemStore GetWorkItemStore(TfsTeamProjectCollection collection)
        {
            return collection.GetService<WorkItemStore>();
        }
    }
}