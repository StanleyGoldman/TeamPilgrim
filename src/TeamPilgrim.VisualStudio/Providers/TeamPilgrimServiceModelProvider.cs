using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NLog;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class TeamPilgrimServiceModelProvider : ITeamPilgrimServiceModelProvider
    {
        private static readonly Logger Logger = TeamPilgrimLogManager.Instance.GetCurrentClassLogger();

        private readonly ITeamPilgrimTfsService _teamPilgrimTfsService;

        public Exception LastException { get; private set; }

        public void ClearLastException()
        {
            LastException = null;
        }

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
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

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
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

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
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

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
                Logger.DebugException(ex);
                LastException = ex;
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
                Logger.DebugException(ex);
                LastException = ex;
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
                Logger.DebugException(ex);
                LastException = ex;
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
                Logger.DebugException(ex);
                LastException = ex;
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
                LastException = ex;
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
                Logger.DebugException(ex);
                LastException = ex;
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
                Logger.DebugException(ex);
                LastException = ex;
            }

            workspace = null;
            return false;
        }

        public bool TryCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote = null, WorkItemCheckinInfo[] workItemChanges = null, PolicyOverrideInfo policyOverride = null)
        {
            try
            {
                _teamPilgrimTfsService.WorkspaceCheckin(workspace, changes, comment, checkinNote, workItemChanges, policyOverride);
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

            return false;
        }

        public bool TryShelve(Workspace workspace, Shelveset shelveset, PendingChange[] pendingChanges, ShelvingOptions shelvingOptions)
        {
            try
            {
                _teamPilgrimTfsService.WorkspaceShelve(workspace, shelveset, pendingChanges, shelvingOptions);
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

            return false;
        }

        public bool TryGetVersionControlServer(out VersionControlServer versionControlServer, TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            try
            {
                versionControlServer = _teamPilgrimTfsService.GetVersionControlServer(tfsTeamProjectCollection);
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

            versionControlServer = null;
            return false;
        }

        public bool TryEvaluateCheckin(out CheckinEvaluationResult checkinEvaluationResult, Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote = null, WorkItemCheckinInfo[] workItemChanges = null)
        {
            try
            {
                checkinEvaluationResult = _teamPilgrimTfsService.EvaluateCheckin(workspace, changes, comment, checkinNote, workItemChanges);
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }
            
            checkinEvaluationResult = null;
            return false;
        }

        public bool TryGetAllConflicts(out Conflict[] conflicts, Workspace workspace)
        {
            try
            {
                conflicts = _teamPilgrimTfsService.GetAllConflicts(workspace);
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
            }

            conflicts = null;
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
                Logger.DebugException(ex);
                LastException = ex;
            }
            
            pendingChanges = null;
            return false;
        }

        public bool TryGetPendingChanges(out PendingChange[] pendingChanges, Workspace workspace, string[] items)
        {
            try
            {
                pendingChanges = workspace.GetPendingChanges(items);
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugException(ex);
                LastException = ex;
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
                Logger.DebugException(ex);
                LastException = ex;
            }

            workItemCollection = null;
            return false;
        }
    }
}
