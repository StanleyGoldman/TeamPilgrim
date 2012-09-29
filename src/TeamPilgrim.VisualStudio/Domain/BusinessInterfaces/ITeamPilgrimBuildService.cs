using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimBuildService
    {
        PilgrimBuildDetail[] QueryBuilds(string teamProject);
    }
}