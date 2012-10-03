using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
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

        public BuildDefinitionWrapper[] QueryBuildDefinitions(string teamProject)
        {
            return _buildServer
                .QueryBuildDefinitions(teamProject)
                .Select(definition => new BuildDefinitionWrapper(definition))
                .ToArray();
        }

        public BuildDetailWrapper[] QueryBuildDetails(string teamProject)
        {
            return _buildServer
                .QueryBuilds(teamProject)
                .Select(detail => new BuildDetailWrapper(detail))
                .ToArray();
        }
    }
}
