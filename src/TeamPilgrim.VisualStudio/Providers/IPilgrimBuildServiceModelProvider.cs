using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimBuildServiceModelProvider
    {
        bool TryGetBuildsByProjectName(out PilgrimBuildDetail[] pilgrimBuildDetails, string teamProject);
    }
}