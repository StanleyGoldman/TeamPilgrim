using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimBuildModel : BaseModel
    {
        private IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private IPilgrimBuildServiceModelProvider _buildServiceModelProvider;
        private PilgrimProjectCollection _collection;

        public PilgrimBuildModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, IPilgrimBuildServiceModelProvider buildServiceModelProvider, PilgrimProjectCollection collection)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _buildServiceModelProvider = buildServiceModelProvider;
            _collection = collection;
        }
    }
}
