using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface ITeamPilgrimServiceModelProvider
    {
        Exception LastException { get; }

        void ClearLastException();

        bool TryGetCollections(out TfsTeamProjectCollection[] collections);

        bool TryGetCollection(out TfsTeamProjectCollection collection, Uri tpcAddress);

        bool TryGetProjects(out Project[] projects, Uri tpcAddress);

        bool TryGetProjects(out Project[] projects, TfsTeamProjectCollection tfsTeamProjectCollection);

        bool TryDeleteQueryItem(out bool result, TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryItemId);

        bool TryGetBuildDefinitionsByProjectName(out IBuildDefinition[] buildDefinitions, TfsTeamProjectCollection collection, string projectName);

        bool TryCloneQueryDefinition(out IBuildDefinition buildDefinition, TfsTeamProjectCollection collection, Project project, IBuildDefinition definition);

        bool TryDeleteBuildDefinition(IBuildDefinition definition);

        bool TryAddNewQueryFolder(out QueryFolder childFolder, TfsTeamProjectCollection tfsTeamProjectCollection, Project teamProject, Guid parentFolderId);

        bool TryGetLocalWorkspaceInfos(out WorkspaceInfo[] workspaceInfos, Guid? projectCollectionId = null);

        bool TryGetWorkspace(out Workspace workspace, WorkspaceInfo workspaceInfo, TfsTeamProjectCollection tfsTeamProjectCollection);

        bool TryCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote = null, WorkItemCheckinInfo[] workItemChanges = null, PolicyOverrideInfo policyOverride = null);

        bool TryGetPendingChanges(out PendingChange[] pendingChanges, Workspace workspace);

        bool TryGetQueryDefinitionWorkItemCollection(out WorkItemCollection workItemCollection, TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);

        bool TryEvaluateCheckin(out CheckinEvaluationResult checkinEvaluationResult, Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote = null, WorkItemCheckinInfo[] workItemChanges = null);

        bool TryGetAllConflicts(out Conflict[] conflicts, Workspace workspace);

        bool TryGetPendingChanges(out PendingChange[] pendingChanges, Workspace workspace, string[] items);

        bool TryShelve(Workspace workspace, Shelveset shelveset, PendingChange[] pendingChanges, ShelvingOptions shelvingOptions);

        bool TryWorkspaceQueryShelvedChanges(Workspace workspace, out PendingSet[] pendingSets, string shelvesetName, string shelvesetOwner, ItemSpec[] itemSpecs);

        bool TryQueryShelvesets(TfsTeamProjectCollection tfsTeamProjectCollection, out Shelveset[] shelvesets, string shelvesetName = null, string shelvesetOwner = null);

        bool TryWorkspaceUnshelve(Workspace workspace, out Shelveset shelveset, string shelvesetName, string shelvesetOwner, ItemSpec[] items = null);

        bool TryDeleteShelveset(TfsTeamProjectCollection tfsTeamProjectCollection, string shelvesetName, string shelvesetOwner);

        bool TryGetOneHopQueryDefinitionWorkItemLinkInfo(out WorkItemLinkInfo[] workItemLinkInfos, TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);
    }
}
