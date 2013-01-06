using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Messages.ShowSelectWorkItemQueryDialog
{
    public interface IShowSelectWorkItemQueryDialogSender
    {
        WorkItemQueryDefinitionModel SelectedWorkItemQueryDefinition { get; set; }
    }
}