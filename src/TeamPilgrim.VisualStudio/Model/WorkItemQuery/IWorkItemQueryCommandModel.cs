using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public interface IWorkItemQueryCommandModel
    {
        RelayCommand<WorkItemQueryChildModel> DeleteQueryItemCommand { get; }
        RelayCommand<WorkItemQueryChildModel> OpenSeurityDialogCommand { get; }

        RelayCommand<WorkItemQueryFolderModel> NewQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryFolderModel> NewQueryFolderCommand { get; }

        RelayCommand<WorkItemQueryDefinitionModel> OpenQueryDefinitionCommand { get; }
        RelayCommand<WorkItemQueryDefinitionModel> EditQueryDefinitionCommand { get; }
    }
}
