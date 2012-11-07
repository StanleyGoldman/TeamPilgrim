using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer
{
    public class TeamMembersModel : BaseModel
    {
        public TeamMembersModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
        }
    }
}