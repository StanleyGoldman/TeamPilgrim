using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.Business.Services
{
    public class TeamPilgrimBuildService : ITeamPilgrimBuildService
    {
        private IBuildServer _buildServer;

        public TeamPilgrimBuildService(IBuildServer buildServer)
        {
            _buildServer = buildServer;
        }
    }
}
