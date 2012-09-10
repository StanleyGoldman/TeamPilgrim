using JustAProgrammer.TeamPilgrim.Domain.Entities;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Providers
{
    public interface IPilgrimModelProvider
    {
        bool TryGetCollections(out PilgrimProjectCollection[] collections);
    }
}
