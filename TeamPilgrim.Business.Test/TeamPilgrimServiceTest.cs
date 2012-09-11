using System.Linq;
using JustAProgrammer.TeamPilgrim.Business.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JustAProgrammer.TeamPilgrim.Business.Test
{
    [TestClass]
    public class TeamPilgrimServiceTest
    {
        private TeamPilgrimService _teamPilgrimService;

        [TestInitialize]
        public void Initialize()
        {
            _teamPilgrimService = new TeamPilgrimService();
        }

        [TestMethod]
        public void Test()
        {
            var pilgrimProjectCollections = _teamPilgrimService.GetPilgrimProjectCollections();
            var pilgrimProjectCollection = pilgrimProjectCollections.First();

            var pilgrimProjects = _teamPilgrimService.GetPilgrimProjects(pilgrimProjectCollection.ProjectCollection.Uri);
        }
    }
}
