using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimTfsService
    {
        TfsTeamProjectCollection[] GetProjectCollections();
        TfsTeamProjectCollection GetProjectCollection(Uri uri);

        Project[] GetProjects(Uri tpcAddress);
        RegisteredProjectCollection[] GetRegisteredProjectCollections();

        QueryFolder AddNewQueryFolder(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid parentFolderId);
        bool DeleteQueryItem(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryItemId);

        IBuildDefinition[] QueryBuildDefinitions(TfsTeamProjectCollection collection, string teamProject);
        IBuildDetail[] QueryBuildDetails(TfsTeamProjectCollection collection, string teamProject);
		
		IBuildDefinition CloneBuildDefinition(TfsTeamProjectCollection collection, string projectName, IBuildDefinition sourceDefinition);
        void DeleteBuildDefinition(IBuildDefinition buildDefinition);
        WorkspaceInfo[] GetLocalWorkspaceInfo(Guid? projectCollectionId = null);
        Workspace GetWorkspace(WorkspaceInfo workspaceInfo, TfsTeamProjectCollection tfsTeamProjectCollection);
        void WorkspaceCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote, WorkItemCheckinInfo[] workItemChanges, PolicyOverrideInfo policyOverride);
        WorkItemCollection GetQueryDefinitionWorkItemCollection(TfsTeamProjectCollection collection, QueryDefinition queryDefinition, string projectName);
        CheckinEvaluationResult EvaluateCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote, WorkItemCheckinInfo[] workItemChanges);
        Conflict[] GetAllConflicts(Workspace workspace);
    }
}