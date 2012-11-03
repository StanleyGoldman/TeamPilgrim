using System;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public delegate void ActiveProjectContextChanged(ProjectContextExt projectContext);
    
    public interface ITeamPilgrimVsService
    {
        event ActiveProjectContextChanged ActiveProjectContextChangedEvent;
        ProjectContextExt ActiveProjectContext { get; }

        void OpenSourceControl(string projectName);

        void NewQueryDefinition(Project project, QueryFolder parent);
        void OpenQueryDefinition(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId);
        void EditQueryDefinition(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId);
        void CloseQueryDefinitionFrames(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId);

        void ViewBuilds(string projectName, string buildDefinition, string qualityFilter, DateFilter dateFilter);
        void OpenBuildDefinition(Uri buildDefinitionUri);
        void QueueBuild(string projectName, Uri buildDefinitionUri);
        void OpenProcessFileLocation(Uri buildDefinitionUri);
        
        void TfsConnect();
        void NewBuildDefinition(string projectName);
        void OpenControllerAgentManager(string projectName);
        void OpenQualityManager(string projectName);
        void OpenBuildSecurityDialog(string projectName, string projectUri);
        void OpenBuildDefinitionSecurityDialog(string projectName, string projectUri, string definitionName, string definitionUri);

        void NewWorkItem(TfsTeamProjectCollection projectCollection, string projectName, string typeName);
        void GoToWorkItem();
    }
}
