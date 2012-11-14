using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class PendingChangeModel : BaseModel
    {
        public IPendingChangeCommandModel PendingChangeCommand { get; private set; }

        public PendingChange Change { get; private set; }

        public PendingChangeModel(IPendingChangeCommandModel pendingChangeCommandModel, PendingChange change)
        {
            PendingChangeCommand = pendingChangeCommandModel;
            Change = change;
        }

        private bool _includeChange;

        public bool IncludeChange
        {
            get
            {
                return _includeChange;
            }
            set
            {
                if (_includeChange == value) return;

                _includeChange = value;

                SendPropertyChanged("IncludeChange");
            }
        }
    }
}