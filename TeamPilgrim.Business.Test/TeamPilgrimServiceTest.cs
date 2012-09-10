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
    }
}
