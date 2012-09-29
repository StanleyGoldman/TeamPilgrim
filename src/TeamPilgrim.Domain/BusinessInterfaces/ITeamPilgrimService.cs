using System;
using JustAProgrammer.TeamPilgrim.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimService
    {
        PilgrimProjectCollection[] GetPilgrimProjectCollections();

        PilgrimProject[] GetPilgrimProjects(Uri tpcAddress);
        ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress);
    }
}