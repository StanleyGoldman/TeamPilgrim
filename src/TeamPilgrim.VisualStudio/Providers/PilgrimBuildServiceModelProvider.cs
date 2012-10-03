using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class PilgrimBuildServiceModelProvider : IPilgrimBuildServiceModelProvider
    {
        private readonly ITeamPilgrimBuildService _teamPilgrimBuildService;

        public PilgrimBuildServiceModelProvider(ITeamPilgrimBuildService teamPilgrimBuildService)
        {
            _teamPilgrimBuildService = teamPilgrimBuildService;
        }

        public bool TryGetBuildDefinitionsByProjectName(out BuildDefinitionWrapper[] buildDefinitions, string teamProject)
        {
            try
            {
                buildDefinitions = _teamPilgrimBuildService.QueryBuildDefinitions(teamProject);
                return true;
            }
            catch (Exception) { }
            
            buildDefinitions = null;
            return false;
        }
    }
}
