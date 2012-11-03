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
        public TfsTeamProjectCollection GetProjectCollection(Uri uri)
        {
            if (uri == null)
                return null;

            var tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri);
            tfsTeamProjectCollection.Authenticate();

            return tfsTeamProjectCollection;
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

        private WorkItemStore GetWorkItemStore(Uri tpcAddress)
        {
            return GetWorkItemStore(GetProjectCollection(tpcAddress));
        }

        public IBuildDefinition CloneBuildDefinition(TfsTeamProjectCollection collection, Project project, IBuildDefinition sourceDefinition)
        {
            var buildServer = collection.GetService<IBuildServer>();
            var clonedDefinition = buildServer.CreateBuildDefinition(project.Name);
            clonedDefinition.CopyFrom(sourceDefinition);
            clonedDefinition.Name = string.Format("Copy of {0}", clonedDefinition.Name);
            clonedDefinition.Save();
            return clonedDefinition;
        }

        private WorkItemStore GetWorkItemStore(TfsTeamProjectCollection collection)
        {
            return collection.GetService<WorkItemStore>();
        }

        public IBuildDefinition[] QueryBuildDefinitions(TfsTeamProjectCollection collection, string teamProject)
        {
            return collection.GetService<IBuildServer>()
                .QueryBuildDefinitions(teamProject)
                .ToArray();
        }

        public IBuildDetail[] QueryBuildDetails(TfsTeamProjectCollection collection, string teamProject)
        {
            return collection.GetService<IBuildServer>()
                .QueryBuilds(teamProject)
                .ToArray();
        }
    }
}