using System;
using JustAProgrammer.TeamPilgrim.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimModelProvider
    {
        bool TryGetCollections(out PilgrimProjectCollection[] collections);
        
        bool TryGetProjects(out PilgrimProject[] projects, Uri tpcAddress);
        
        bool TryGetBuildServer(out PilgrimBuildServer pilgrimBuildServer, Uri tpcAddress);
    }
}
