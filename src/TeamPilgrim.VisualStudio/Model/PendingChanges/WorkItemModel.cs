using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkItemModel : BaseModel
    {
        public WorkItem WorkItem { get; private set; }

        public WorkItemModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, WorkItem workItem)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            WorkItem = workItem;
        }
    }
}