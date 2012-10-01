using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimServiceModelProvider
    {
        bool TryGetCollections(out TfsTeamProjectCollection[] collections);
        
        bool TryGetProjects(out Project[] projects, Uri tpcAddress);

        bool TryGetBuildServiceProvider(out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress);

        bool TryGetProjectsAndBuildServiceProvider(out Project[] projects, out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress);
    }
}
