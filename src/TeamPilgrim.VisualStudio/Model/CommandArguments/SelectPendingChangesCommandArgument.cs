using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments
{
    public class SelectPendingChangesCommandArgument
    {
        public PendingChangeModel[] Collection { get; set; }
        public bool Value { get; set; }
    }
}