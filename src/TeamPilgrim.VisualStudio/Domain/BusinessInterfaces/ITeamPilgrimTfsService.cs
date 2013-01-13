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
        Project[] GetProjects(TfsTeamProjectCollection tfsTeamProjectCollection);

        RegisteredProjectCollection[] GetRegisteredProjectCollections();

        QueryFolder AddNewQueryFolder(TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid parentFolderId);
        bool DeleteQueryItem(TfsTeamProjectCollection tfsTeamProjectCollection, Project teamProject, Guid queryItemId);

        IBuildDefinition[] QueryBuildDefinitions(TfsTeamProjectCollection tfsTeamProjectCollection, string teamProject);
        IBuildDetail[] QueryBuildDetails(TfsTeamProjectCollection tfsTeamProjectCollection, string teamProject);

        IBuildDefinition CloneBuildDefinition(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName, IBuildDefinition sourceDefinition);
        void DeleteBuildDefinition(IBuildDefinition buildDefinition);
        WorkspaceInfo[] GetLocalWorkspaceInfo(Guid? projectCollectionId = null);
        Workspace GetWorkspace(WorkspaceInfo workspaceInfo, TfsTeamProjectCollection tfsTeamProjectCollection);
        void WorkspaceCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote, WorkItemCheckinInfo[] workItemChanges, PolicyOverrideInfo policyOverride);
        CheckinEvaluationResult EvaluateCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote, WorkItemCheckinInfo[] workItemChanges);
        Conflict[] GetAllConflicts(Workspace workspace);
        VersionControlServer GetVersionControlServer(TfsTeamProjectCollection tfsTeamProjectCollection);
        
        PendingSet[] WorkspaceQueryShelvedChanges(Workspace workspace, string shelvesetName, string shelvesetOwner, ItemSpec[] itemSpecs);
        void WorkspaceShelve(Workspace workspace, Shelveset shelveset, PendingChange[] pendingChanges, ShelvingOptions shelvingOptions);
        Shelveset WorkspaceUnshelve(Workspace workspace, string shelvesetName, string shelvesetOwner, ItemSpec[] items = null);
       
        Shelveset[] QueryShelvesets(TfsTeamProjectCollection tfsTeamProjectCollection, string shelvesetName = null, string shelvesetOwner = null);
        void DeleteShelveset(TfsTeamProjectCollection tfsTeamProjectCollection, string shelvesetName, string shelvesetOwner);

        WorkItemLinkInfo[] GetOneHopQueryDefinitionWorkItemLinkInfo(TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);

        WorkItemCollection GetQueryDefinitionWorkItemCollection(TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);
        WorkItemCollection GetListQueryDefinitionWorkItemCollection(TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);
        WorkItemCollection GetOneHopQueryDefinitionWorkItemCollection(TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);
    }
}