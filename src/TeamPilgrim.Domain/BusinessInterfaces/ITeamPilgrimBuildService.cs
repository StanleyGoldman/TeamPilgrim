using System.Collections.Generic;
using JustAProgrammer.TeamPilgrim.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimBuildService
    {
        PilgrimBuildDetail[] QueryBuilds(string teamProject);
    }
}