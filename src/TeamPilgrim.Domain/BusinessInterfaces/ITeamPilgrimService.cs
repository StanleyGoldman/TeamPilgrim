using System;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimService
    {
        PilgrimProjectCollection[] GetPilgrimProjectCollections();

        PilgrimProject[] GetPilgrimProjects(Uri tpcAddress);
        PilgrimProject[] GetPilgrimProjects(TfsTeamProjectCollection tfsTeamProjectCollection);
        ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress);
        ITeamPilgrimBuildService GetTeamPilgrimBuildService(TfsTeamProjectCollection collection);
    }
}