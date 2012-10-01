using System;
using System.Collections.Concurrent;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class TeamPilgrimService : ITeamPilgrimService
    {
        private readonly ConcurrentDictionary<string, TfsTeamProjectCollection> _projectCollectionDictionary = new ConcurrentDictionary<string, TfsTeamProjectCollection>();

        private TfsTeamProjectCollection GetTfsTeamProjectCollection(Uri uri)
        {
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

        public TfsTeamProjectCollection[] GetProjectCollections()
        {
            RegisteredProjectCollection[] registeredProjectCollections = GetRegisteredProjectCollections();

            return registeredProjectCollections.Select(collection => new TfsTeamProjectCollection(collection)).ToArray();
        }

        public Project[] GetProjects(Uri tpcAddress)
        {
            return GetProjects(GetTfsTeamProjectCollection(tpcAddress));
        }

        private Project[] GetProjects(TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            var workItemStore = new WorkItemStore(tfsTeamProjectCollection);

            return workItemStore.Projects.Cast<Project>().ToArray();
        }

        public ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress)
        {
            var collection = GetTfsTeamProjectCollection(tpcAddress);
            return GetTeamPilgrimBuildService(collection);
        }

        private ITeamPilgrimBuildService GetTeamPilgrimBuildService(TfsTeamProjectCollection collection)
        {
            var buildServer = collection.GetService<IBuildServer>();

            return new TeamPilgrimBuildService(buildServer);
        }
    }
}