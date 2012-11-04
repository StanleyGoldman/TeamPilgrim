using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkspaceInfoModel : BaseModel
    {
        public WorkspaceInfo WorkspaceInfo { get; private set; }

        public WorkspaceInfoModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, WorkspaceInfo workspaceInfo)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            WorkspaceInfo = workspaceInfo;
        }
    }
}