using System.Linq;
using JustAProgrammer.TeamPilgrim.Business.Services;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using NUnit.Framework;

namespace JustAProgrammer.TeamPilgrim.Business.Tests
{
    [TestFixture]
    public class TeamPilgrimServiceTest
    {
        private TeamPilgrimService _teamPilgrimService;

        [SetUp]
        public void Initialize()
        {
            _teamPilgrimService = new TeamPilgrimService();
        }

        [Test]
        public void ManualTest()
        {
            var pilgrimProjectCollections = _teamPilgrimService.GetPilgrimProjectCollections();
            var pilgrimProjectCollection = pilgrimProjectCollections.First();

            var teamPilgrimBuildService = _teamPilgrimService.GetTeamPilgrimBuildService(pilgrimProjectCollection.ProjectCollection.Uri);

            var pilgrimProjects = _teamPilgrimService.GetPilgrimProjects(pilgrimProjectCollection.ProjectCollection.Uri);
            PilgrimProject pilgrimProject = pilgrimProjects.First();

            teamPilgrimBuildService.QueryBuilds(pilgrimProject.Project.Name);
        }
    }
}
