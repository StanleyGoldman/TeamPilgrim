using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class TeamPilgrimBuildService : ITeamPilgrimBuildService
    {
        private readonly IBuildServer _buildServer;

        public TeamPilgrimBuildService(IBuildServer buildServer)
        {
            _buildServer = buildServer;
        }

        public IBuildDetail[] QueryBuilds(string teamProject)
        {
            return _buildServer.QueryBuilds(teamProject);
        }
    }
}
