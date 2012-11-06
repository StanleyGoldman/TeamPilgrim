using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class SelectWorkItemQueryModel : BaseModel
    {
        public ProjectCollectionModel ActiveProjectCollectionModel { get; private set; }

        public SelectWorkItemQueryModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionModel activeProjectCollectionModel)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ActiveProjectCollectionModel = activeProjectCollectionModel;
        }
    }
}
