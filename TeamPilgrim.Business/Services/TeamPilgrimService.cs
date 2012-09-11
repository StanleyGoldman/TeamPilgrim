using System;
using System.Collections.Generic;
using System.Linq;
using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.Business.Services
{
    public class TeamPilgrimService : ITeamPilgrimService
    {
        public PilgrimProjectCollection[] GetPilgrimProjectCollections()
        {
            var registeredProjectCollections = RegisteredTfsConnections.GetProjectCollections();

            return registeredProjectCollections.Select(collection => new PilgrimProjectCollection
                {
                    ProjectCollection = collection
                }).ToArray();
        }

        public PilgrimProject[] GetPilgrimProjects(Uri tpcAddress)
        {
            var tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tpcAddress);
            
            tfsTeamProjectCollection.Authenticate();

            var workItemStore = new WorkItemStore(tfsTeamProjectCollection);

            return (from object project in workItemStore.Projects
                    select new PilgrimProject

                        {
                            Project = (Project) project
                        }).ToArray();
        }
    }
}