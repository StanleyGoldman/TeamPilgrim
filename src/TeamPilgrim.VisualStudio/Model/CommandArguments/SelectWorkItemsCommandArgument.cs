using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments
{
    public class SelectWorkItemsCommandArgument
    {
        public WorkItemModel[] Collection { get; set; }
        public bool Value { get; set; }
    }
}