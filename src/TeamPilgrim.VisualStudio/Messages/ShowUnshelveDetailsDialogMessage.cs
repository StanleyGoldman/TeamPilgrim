using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Messages
{
    public class ShowUnshelveDetailsDialogMessage:BaseMessage
    {
        public UnshelveDetailsServiceModel UnshelveDetailsServiceModel { get; set; }
    }
}