using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public interface IWorkItemQueryCommandModel
    {
        RelayCommand<WorkItemQueryFolderModel> NewQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryFolderModel> NewQueryFolderCommand { get; }

        RelayCommand<WorkItemQueryDefinitionModel> OpenQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryDefinitionModel> EditQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryDefinitionModel> DeleteQueryDefinitionCommand { get; }
    }
}
