using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimBuildService
    {
        IBuildDetail[] QueryBuilds(string teamProject);
    }
}