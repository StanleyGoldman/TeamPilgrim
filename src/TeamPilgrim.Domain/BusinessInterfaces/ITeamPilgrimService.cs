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

        PilgrimBuildServer GetPilgrimBuildServer(Uri tpcAddress);
        PilgrimBuildServer GetPilgrimBuildServer(TfsTeamProjectCollection collection);
    }
}