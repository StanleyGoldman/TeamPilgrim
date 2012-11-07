using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class TeamPilgrimServiceModelProvider : ITeamPilgrimServiceModelProvider
    {
        private readonly ITeamPilgrimTfsService _teamPilgrimTfsService;

        public TeamPilgrimServiceModelProvider()
        {
            _teamPilgrimTfsService = new TeamPilgrimTfsService();
        }

        public bool TryGetCollections(out TfsTeamProjectCollection[] collections)
        {
            try
            {
                collections = _teamPilgrimTfsService.GetProjectCollections();
                return true;
            }
            catch (Exception) { }

            collections = null;
            return false;
        }

        public bool TryGetCollection(out TfsTeamProjectCollection collection, Uri tpcAddress)
        {
            try
            {
                collection = _teamPilgrimTfsService.GetProjectCollection(tpcAddress);
                return true;
            }
            catch (Exception) { }

            collection = null;
            return false;
        }

        public bool TryGetProjects(out Project[] projects, Uri tpcAddress)
        {
            try
            {
                projects = _teamPilgrimTfsService.GetProjects(tpcAddress);
                return true;
            }
            catch (Exception) { }

            projects = null;
            return false;
        }

        public bool TryDeleteQueryItem(out bool result, TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryItemId)
        {
            try
            {
                result = _teamPilgrimTfsService.DeleteQueryItem(teamProjectCollection, teamProject, queryItemId);
                return true;
            }
            catch (Exception ex)
            {

            }

            result = false;
            return false;
        }

        public bool TryGetBuildDefinitionsByProjectName(out IBuildDefinition[] buildDefinitions, TfsTeamProjectCollection collection, string projectName)
        {
            try
            {
                buildDefinitions = _teamPilgrimTfsService.QueryBuildDefinitions(collection, projectName);
                return true;
            }
            catch (Exception ex)
            {

            }

            buildDefinitions = null;
            return false;
        }

        public bool TryCloneQueryDefinition(out IBuildDefinition buildDefinition, TfsTeamProjectCollection collection, Project project, IBuildDefinition definition)
        {
            try
            {
                buildDefinition = _teamPilgrimTfsService.CloneBuildDefinition(collection, project.Name, definition);
                return true;
            }
            catch (Exception ex)
            {

            }

            buildDefinition = null;
            return false;
        }

        public bool TryDeleteBuildDefinition(IBuildDefinition definition)
        {
            try
            {
                _teamPilgrimTfsService.DeleteBuildDefinition(definition);
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public bool TryAddNewQueryFolder(out QueryFolder childFolder, TfsTeamProjectCollection tfsTeamProjectCollection, Project teamProject, Guid parentFolderId)
        {
            try
            {
                childFolder = _teamPilgrimTfsService.AddNewQueryFolder(tfsTeamProjectCollection, teamProject, parentFolderId);
                return true;
            }
            catch (Exception ex)
            {

            }

            childFolder = null;
            return false;
        }

        public bool TryGetLocalWorkspaceInfos(out WorkspaceInfo[] workspaceInfos, Guid? projectCollectionId = null)
        {
            try
            {
                workspaceInfos = _teamPilgrimTfsService.GetLocalWorkspaceInfo(projectCollectionId);
                return true;
            }
            catch (Exception ex)
            {

            }

            workspaceInfos = null;
            return false;
        }

        public bool TryGetWorkspace(out Workspace workspace, WorkspaceInfo workspaceInfo, TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            try
            {
                workspace = _teamPilgrimTfsService.GetWorkspace(workspaceInfo, tfsTeamProjectCollection);
                return true;
            }
            catch (Exception ex)
            {

            }

            workspace = null;
            return false;
        }

        public bool TryWorkspaceCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote = null, WorkItemCheckinInfo[] workItemChanges = null, PolicyOverrideInfo policyOverride = null)
        {
            try
            {
                _teamPilgrimTfsService.WorkspaceCheckin(workspace, changes, comment, checkinNote, workItemChanges, policyOverride);
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public bool TryGetPendingChanges(out PendingChange[] pendingChanges, Workspace workspace)
        {
            try
            {
                pendingChanges = workspace.GetPendingChanges();
                return true;
            }
            catch (Exception ex)
            {

            }
            
            pendingChanges = null;
            return false;
        }

        public bool TryGetQueryDefinitionWorkItemCollection(out WorkItemCollection workItemCollection, TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName)
        {
            try
            {
                workItemCollection = _teamPilgrimTfsService.GetQueryDefinitionWorkItemCollection(tfsTeamProjectCollection, queryDefinition, projectName);
                return true;
            }
            catch (Exception ex)
            {

            }

            workItemCollection = null;
            return false;
        }
    }
}
