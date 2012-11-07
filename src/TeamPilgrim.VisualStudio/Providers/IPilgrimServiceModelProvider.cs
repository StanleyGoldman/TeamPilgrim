using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimServiceModelProvider
    {
        bool TryGetCollections(out TfsTeamProjectCollection[] collections);
        
        bool TryGetCollection(out TfsTeamProjectCollection collection, Uri tpcAddress);
        
        bool TryGetProjects(out Project[] projects, Uri tpcAddress);

        bool TryDeleteQueryItem(out bool result, TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryItemId);

        bool TryGetBuildDefinitionsByProjectName(out IBuildDefinition[] buildDefinitions, TfsTeamProjectCollection collection, string projectName);

        bool TryCloneQueryDefinition(out IBuildDefinition buildDefinition, TfsTeamProjectCollection collection, Project project, IBuildDefinition definition);
		
        bool TryDeleteBuildDefinition(IBuildDefinition definition);

        bool TryAddNewQueryFolder(out QueryFolder childFolder, TfsTeamProjectCollection tfsTeamProjectCollection, Project teamProject, Guid parentFolderId);
      
        bool TryGetLocalWorkspaceInfos(out WorkspaceInfo[] workspaceInfos, Guid? projectCollectionId = null);
       
	    bool TryGetWorkspace(out Workspace workspace, WorkspaceInfo workspaceInfo, TfsTeamProjectCollection tfsTeamProjectCollection);
        
        bool TryWorkspaceCheckin(Workspace workspace, PendingChange[] changes, string comment, CheckinNote checkinNote = null, WorkItemCheckinInfo[] workItemChanges = null, PolicyOverrideInfo policyOverride = null);
        
        bool TryGetPendingChanges(out PendingChange[] pendingChanges, Workspace workspace);
        
		bool TryGetQueryDefinitionWorkItemCollection(out WorkItemCollection workItemCollection, TfsTeamProjectCollection tfsTeamProjectCollection, QueryDefinition queryDefinition, string projectName);
    }
}
