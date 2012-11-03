using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public interface IWorkItemQueryCommandModel
    {
        RelayCommand<WorkItemQueryDefinitionModel> OpenQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryDefinitionModel> EditQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryDefinitionModel> DeleteQueryDefinitionCommand { get; }
    }
}
