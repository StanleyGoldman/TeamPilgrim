using System.Linq;
using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.Business.Services
{
    public class TeamPilgrimService : ITeamPilgrimService
    {
        public PilgrimProjectCollection[] GetPilgrimProjectCollections()
        {
            var registeredProjectCollections = RegisteredTfsConnections.GetProjectCollections();

            return registeredProjectCollections.Select(collection => new PilgrimProjectCollection
                {
                    RegisteredProjectCollection = collection
                }).ToArray();
        }
    }
}
