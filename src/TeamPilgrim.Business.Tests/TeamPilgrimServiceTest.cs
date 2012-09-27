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

        public void Test()
        {
            var pilgrimProjectCollections = _teamPilgrimService.GetPilgrimProjectCollections();
            var pilgrimProjectCollection = pilgrimProjectCollections.First();

            var pilgrimProjects = _teamPilgrimService.GetPilgrimProjects(pilgrimProjectCollection.ProjectCollection.Uri);

            PilgrimProject pilgrimProject = pilgrimProjects.First();
        }
    }
}
