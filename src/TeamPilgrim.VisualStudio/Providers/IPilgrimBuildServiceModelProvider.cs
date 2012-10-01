using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimBuildServiceModelProvider
    {
        bool TryGetBuildsByProjectName(out IBuildDetail[] pilgrimBuildDetails, string teamProject);
    }
}