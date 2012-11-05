using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class PendingChangeModel : BaseModel
    {
        public PendingChange Change { get; private set; }

        public PendingChangeModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, PendingChange change)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            Change = change;
        }

        private bool _includeChange;

        public bool IncludeChange
        {
            get
            {
                return _includeChange;
            }
            private set
            {
                if (_includeChange == value) return;

                _includeChange = value;

                SendPropertyChanged("IncludeChange");
            }
        }
    }
}