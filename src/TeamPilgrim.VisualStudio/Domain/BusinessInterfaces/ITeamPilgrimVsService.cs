using System;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.TeamFoundation;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public delegate void ActiveProjectContextChanged(ProjectContextExt projectContext);
    
    public interface ITeamPilgrimVsService
    {
        event ActiveProjectContextChanged ActiveProjectContextChangedEvent;
        ProjectContextExt ActiveProjectContext { get; }

        void OpenSourceControl(string projectName);
        
        void OpenQueryDefinition(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId);
        void EditQueryDefinition(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId);
        void CloseQueryDefinitionFrames(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId);
        
        void OpenBuildDefinition(Uri uri);
        void ViewBuilds(string name, string s, string empty, DateFilter today);
    }
}
