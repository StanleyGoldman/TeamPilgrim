using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class PilgrimBuildServiceModelProvider : IPilgrimBuildServiceModelProvider
    {
        private readonly ITeamPilgrimBuildService _teamPilgrimBuildService;

        public PilgrimBuildServiceModelProvider(ITeamPilgrimBuildService teamPilgrimBuildService)
        {
            _teamPilgrimBuildService = teamPilgrimBuildService;
        }
    }
}
