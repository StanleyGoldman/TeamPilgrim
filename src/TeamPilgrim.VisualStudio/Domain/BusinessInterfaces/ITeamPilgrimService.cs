﻿using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimService
    {
        TfsTeamProjectCollection[] GetProjectCollections();

        Project[] GetProjects(Uri tpcAddress);
        ITeamPilgrimBuildService GetTeamPilgrimBuildService(Uri tpcAddress);
        RegisteredProjectCollection[] GetRegisteredProjectCollections();
    }
}