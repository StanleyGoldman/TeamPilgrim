using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
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

        public bool DeleteQueryItem(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryItemId)
        {
            var workItemStore = GetWorkItemStore(teamProjectCollection);
            var queryHierarchy = workItemStore.GetQueryHierarchy(teamProject);
            var queryItem = queryHierarchy.Find(queryItemId);

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

        public void DeleteBuildDefinition(IBuildDefinition buildDefinition)
        {
            buildDefinition.Delete();
        }

        public IBuildDefinition CloneBuildDefinition(TfsTeamProjectCollection collection, string projectName, IBuildDefinition sourceDefinition)
        {
            var buildServer = collection.GetService<IBuildServer>();
            var clonedDefinition = buildServer.CreateBuildDefinition(projectName);
            clonedDefinition.CopyFrom(sourceDefinition);
            clonedDefinition.Name = string.Format("Copy of {0}", clonedDefinition.Name);
            clonedDefinition.Save();
            return clonedDefinition;
        }

        private WorkItemStore GetWorkItemStore(TfsTeamProjectCollection collection)
        {
            return collection.GetService<WorkItemStore>();
        }

        private VersionControlServer GetVersionControlServer(TfsTeamProjectCollection collection)
        {
            return collection.ConfigurationServer.GetService<VersionControlServer>();
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

        public QueryFolder AddNewQueryFolder(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid parentFolderId)
        {
            var workItemStore = GetWorkItemStore(teamProjectCollection);
            var queryHierarchy = workItemStore.GetQueryHierarchy(teamProject);
            var loadedParentFolder = queryHierarchy.Find(parentFolderId) as QueryFolder;

            var queryFolder = new QueryFolder("New Folder");

            Debug.Assert(loadedParentFolder != null, "loadedParentFolder != null");
            loadedParentFolder.Add(queryFolder);

            queryHierarchy.Save();

            return queryFolder;
        }

        public WorkItemCollection GetQueryDefinitionWorkItemCollection(TfsTeamProjectCollection collection, QueryDefinition queryDefinition, string projectName)
        {
            var context = new Dictionary<string, string> { { "project", projectName } };
            
            var workItemStore = GetWorkItemStore(collection);

            var query = new Query(workItemStore, queryDefinition.QueryText, context);
            if(query.IsLinkQuery)
            {
                throw new ArgumentException("Link Queries not supported");
            }
            
            return query.RunQuery();
        }

        public WorkspaceInfo[] GetLocalWorkspaceInfo(Guid? projectCollectionId = null)
        {
            IEnumerable<WorkspaceInfo> allLocalWorkspaceInfo = Workstation.Current.GetAllLocalWorkspaceInfo();

            if (projectCollectionId.HasValue)
            {
                allLocalWorkspaceInfo = allLocalWorkspaceInfo.Where(info => info.ServerGuid.Equals(projectCollectionId));
            }

            return allLocalWorkspaceInfo as WorkspaceInfo[] ?? allLocalWorkspaceInfo.ToArray();
        }

        public Workspace GetWorkspace(WorkspaceInfo workspaceInfo, TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            return workspaceInfo.GetWorkspace(tfsTeamProjectCollection);
        }

        public void WorkspaceCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote, WorkItemCheckinInfo[] workItemChanges, PolicyOverrideInfo policyOverride)
        {
            workspace.CheckIn(changes, comment, checkinNote, workItemChanges, policyOverride);
        }
    }
}