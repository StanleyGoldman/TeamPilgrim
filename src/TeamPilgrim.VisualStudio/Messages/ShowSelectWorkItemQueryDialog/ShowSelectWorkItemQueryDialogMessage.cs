using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Messages.ShowSelectWorkItemQueryDialog
{
    public class ShowSelectWorkItemQueryDialogMessage : BaseMessage
    {
        public SelectWorkItemQueryModel SelectWorkItemQueryModel { get; set; }
        
        public IShowSelectWorkItemQueryDialogSender Sender { get; set; }

        public TfsTeamProjectCollection TfsTeamProjectCollection { get; set; }
    }
}