using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimBuildServiceModelProvider
    {
        bool TryGetBuildDefinitionsByProjectName(out BuildDefinitionWrapper[] buildDefinitions, string teamProject);
    }
}