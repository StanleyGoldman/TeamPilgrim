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
    }
}
