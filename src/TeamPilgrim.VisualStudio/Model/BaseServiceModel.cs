using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class BaseServiceModel : BaseModel
    {
        protected readonly ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider;
        protected readonly ITeamPilgrimVsService teamPilgrimVsService;

        public BaseServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
        {
            this.teamPilgrimServiceModelProvider = teamPilgrimServiceModelProvider;
            this.teamPilgrimVsService = teamPilgrimVsService;
        }       
    }
}
