using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
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
            set
            {
                if (_value == value) return;

                _value = value;

                SendPropertyChanged("Value");
            }
        }
    }
}
