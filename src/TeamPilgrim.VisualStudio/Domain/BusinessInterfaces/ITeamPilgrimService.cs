using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimService
    {
        PilgrimProjectCollection[] GetPilgrimProjectCollections();

        PilgrimProject[] GetPilgrimProjects(Uri tpcAddress);
        ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress);
    }
}