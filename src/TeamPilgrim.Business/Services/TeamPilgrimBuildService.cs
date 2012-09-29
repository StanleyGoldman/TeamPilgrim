using System.Linq;
using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.Business.Services
{
    public class TeamPilgrimBuildService : ITeamPilgrimBuildService
    {
        private readonly IBuildServer _buildServer;

        public TeamPilgrimBuildService(IBuildServer buildServer)
        {
            _buildServer = buildServer;
        }

        public PilgrimBuildDetail[] QueryBuilds(string teamProject)
        {
            return _buildServer.QueryBuilds(teamProject).Select(detail => new PilgrimBuildDetail { Detail = detail }).ToArray();
        }
    }
}
