using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class PendingChangeModel : BaseModel
    {
        public PendingChange Change { get; private set; }

        public PendingChangeModel(PendingChange change)
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
            set
            {
                if (_includeChange == value) return;

                _includeChange = value;

                SendPropertyChanged("IncludeChange");
            }
        }
    }
}