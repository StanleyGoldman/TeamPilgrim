using System;
using JustAProgrammer.TeamPilgrim.Business.Services;
using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class PilgrimModelProvider : IPilgrimModelProvider
    {
        private readonly ITeamPilgrimService _teamPilgrimService;

        public PilgrimModelProvider()
        {
            _teamPilgrimService = new TeamPilgrimService();
        }

        public bool TryGetCollections(out PilgrimProjectCollection[] collections)
        {
            try
            {
                collections = _teamPilgrimService.GetPilgrimProjectCollections();
                return true;
            }
            catch (Exception) { }

            collections = null;
            return false;
        }

        public bool TryGetProjects(out PilgrimProject[] projects, Uri tpcAddress)
        {
            try
            {
                projects = _teamPilgrimService.GetPilgrimProjects(tpcAddress);
                return true;
            }
            catch (Exception) { }

            projects = null;
            return false;
        }
    }
}
