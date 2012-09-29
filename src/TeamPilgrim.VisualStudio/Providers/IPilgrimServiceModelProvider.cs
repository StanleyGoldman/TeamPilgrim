using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimServiceModelProvider
    {
        bool TryGetCollections(out PilgrimProjectCollection[] collections);
        
        bool TryGetProjects(out PilgrimProject[] projects, Uri tpcAddress);

        bool TryGetBuildServiceProvider(out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress);

        bool TryGetProjectsAndBuildServiceProvider(out PilgrimProject[] projects, out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress);
    }
}
