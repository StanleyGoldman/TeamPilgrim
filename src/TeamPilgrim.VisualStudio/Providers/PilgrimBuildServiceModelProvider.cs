using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class PilgrimBuildServiceModelProvider : IPilgrimBuildServiceModelProvider
    {
        private readonly ITeamPilgrimBuildService _teamPilgrimBuildService;

        public PilgrimBuildServiceModelProvider(ITeamPilgrimBuildService teamPilgrimBuildService)
        {
            _teamPilgrimBuildService = teamPilgrimBuildService;
        }

        public bool TryGetBuildsByProjectName(out PilgrimBuildDetail[] pilgrimBuildDetails, string teamProject)
        {
            try
            {
                pilgrimBuildDetails = _teamPilgrimBuildService.QueryBuilds(teamProject);
                return true;
            }
            catch (Exception) { }
            
            pilgrimBuildDetails = null;
            return false;
        }
    }
}
