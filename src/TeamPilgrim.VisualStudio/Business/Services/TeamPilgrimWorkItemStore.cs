using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class TeamPilgrimWorkItemStore : ITeamPilgrimWorkItemStore
    {
        private readonly WorkItemStore _workItemStore;

        public TeamPilgrimWorkItemStore(WorkItemStore workItemStore)
        {
            _workItemStore = workItemStore;
        }
    }
}