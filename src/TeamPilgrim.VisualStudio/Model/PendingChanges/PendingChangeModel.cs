using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class PendingChangeModel : BaseModel
    {
        public PendingChangeModel(PendingChange change)
        {
            _change = change;
        }

        private PendingChange _change;
        public PendingChange Change
        {
            get { return _change; }
            set
            {
                if (_change == value) return;

                _change = value;
                
                SendPropertyChanged("IncludeChange");
            }
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