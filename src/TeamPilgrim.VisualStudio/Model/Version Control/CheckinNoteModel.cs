using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class CheckinNoteModel : BaseModel
    {
        public CheckinNoteFieldDefinition CheckinNoteFieldDefinition { get; private set; }

        public CheckinNoteModel(CheckinNoteFieldDefinition checkinNoteFieldDefinition)
        {
            CheckinNoteFieldDefinition = checkinNoteFieldDefinition;
        }

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            private set
            {
                if (_value == value) return;

                _value = value;

                SendPropertyChanged("Value");
            }
        }
    }
}
