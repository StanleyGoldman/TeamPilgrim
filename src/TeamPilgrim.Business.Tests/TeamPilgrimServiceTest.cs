using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
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
            var pilgrimProjectCollections = _teamPilgrimService.GetProjectCollections();
            var pilgrimProjectCollection = pilgrimProjectCollections.First();

            var teamPilgrimBuildService = _teamPilgrimService.GetTeamPilgrimBuildService(pilgrimProjectCollection.Uri);

            var pilgrimProjects = _teamPilgrimService.GetProjects(pilgrimProjectCollection.Uri);
            Project pilgrimProject = pilgrimProjects.First();

            teamPilgrimBuildService.QueryBuilds(pilgrimProject.Name);
        }
    }
}
