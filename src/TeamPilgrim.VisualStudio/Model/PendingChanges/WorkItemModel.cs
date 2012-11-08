using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkItemModel : BaseModel
    {
        public WorkItem WorkItem { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;

                SendPropertyChanged("IsSelected");
            }
        }

        private SelectedWorkItemCheckinActionEnum _workItemCheckinAction;
        public SelectedWorkItemCheckinActionEnum WorkItemCheckinAction
        {
            get
            {
                return _workItemCheckinAction;
            }
            private set
            {
                if (_workItemCheckinAction == value) return;

                _workItemCheckinAction = value;

                SendPropertyChanged("WorkItemCheckinAction");
            }
        }

        public WorkItemModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, WorkItem workItem)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            WorkItem = workItem;
        }
    }
}