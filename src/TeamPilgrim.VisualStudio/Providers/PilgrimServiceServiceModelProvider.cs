using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public class PilgrimServiceServiceModelProvider : IPilgrimServiceModelProvider
    {
        private readonly ITeamPilgrimService _teamPilgrimService;

        public PilgrimServiceServiceModelProvider()
        {
            _teamPilgrimService = new TeamPilgrimService();
        }

        public bool TryGetCollections(out TfsTeamProjectCollection[] collections)
        {
            try
            {
                collections = _teamPilgrimService.GetProjectCollections();
                return true;
            }
            catch (Exception) { }

            collections = null;
            return false;
        }

        public bool TryGetProjects(out Project[] projects, Uri tpcAddress)
        {
            try
            {
                projects = _teamPilgrimService.GetProjects(tpcAddress);
                return true;
            }
            catch (Exception) { }

            projects = null;
            return false;
        }

        public bool TryGetBuildServiceProvider(out IPilgrimBuildServiceModelProvider buildServiceModelProvider, Uri tpcAddress)
        {
            try
            {
                buildServiceModelProvider = new PilgrimBuildServiceModelProvider(_teamPilgrimService.GetTeamPilgrimBuildService(tpcAddress));
                return true;
            }
            catch (Exception) { }

            buildServiceModelProvider = null;
            return false;
        }

        public bool TryGetProjectsAndBuildServiceProvider(out Project[] projects, out IPilgrimBuildServiceModelProvider buildServiceModelProvider, Uri tpcAddress)
        {
            Project[] projects1;
            IPilgrimBuildServiceModelProvider buildServiceModelProvider1 = null;

            var result = TryGetProjects(out projects1, tpcAddress)
                && TryGetBuildServiceProvider(out buildServiceModelProvider1, tpcAddress);

            projects = projects1;
            buildServiceModelProvider = buildServiceModelProvider1;

            return result;
        }
    }
}
