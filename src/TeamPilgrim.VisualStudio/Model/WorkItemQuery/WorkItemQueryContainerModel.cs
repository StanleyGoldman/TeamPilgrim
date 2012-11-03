using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public class WorkItemQueryContainerModel : BaseModel, IWorkItemQueryCommandModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        private readonly TfsTeamProjectCollection _projectCollection;

        private readonly Project _project;

        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public WorkItemQueryContainerModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection projectCollection, Project project)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _projectCollection = projectCollection;
            _project = project;

            NewWorkItemCommand = new RelayCommand<string>(NewWorkItem, CanNewWorkItem);
            GoToWorkItemCommand = new RelayCommand(GoToWorkItem, CanGoToWorkItem);

            NewQueryDefinitionCommand = new RelayCommand<WorkItemQueryFolderModel>(NewQueryDefinition, CanNewQueryDefinition);
            NewQueryFolderCommand = new RelayCommand<WorkItemQueryFolderModel>(NewQueryFolder, CanNewQueryFolder);

            OpenQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(OpenQueryDefinition, CanOpenQueryDefinition);
            EditQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(EditQueryDefinition, CanEditQueryDefinition);
            DeleteQueryItemCommand = new RelayCommand<WorkItemQueryChildModel>(DeleteQueryDefinition, CanDeleteQueryDefinition);

            var queryHierarchy = _project.QueryHierarchy;
            var queryItemModels = queryHierarchy.GetQueryItemViewModels(this);

            QueryItems = new ObservableCollection<WorkItemQueryChildModel>(queryItemModels);
        }

        #region GoToWorkItem Command

        public RelayCommand GoToWorkItemCommand { get; private set; }

        private void GoToWorkItem()
        {
            TeamPilgrimPackage.TeamPilgrimVsService.GoToWorkItem();
        }

        private bool CanGoToWorkItem()
        {
            return true;
        }

        #endregion

        #region NewWorkItem Command

        public RelayCommand<string> NewWorkItemCommand { get; private set; }

        private void NewWorkItem(string workItemType)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.NewWorkItem(_projectCollection, _project.Name, workItemType);
        }

        private bool CanNewWorkItem(string workItemType)
        {
            return true;
        }

        #endregion

        #region NewQueryFolder Command

        public RelayCommand<WorkItemQueryFolderModel> NewQueryFolderCommand { get; private set; }

        private void NewQueryFolder(WorkItemQueryFolderModel workItemQueryDefinitionModel)
        {
            QueryFolder queryFolder;
            QueryFolder parentFolder = workItemQueryDefinitionModel.QueryFolder;
            if (_pilgrimServiceModelProvider.TryAddNewQueryFolder(out queryFolder, _projectCollection, _project, parentFolder.Id))
            {
                workItemQueryDefinitionModel.QueryItems.Insert(0, new WorkItemQueryFolderModel(queryFolder, this, new WorkItemQueryChildModel[0]));
            }
        }

        private bool CanNewQueryFolder(WorkItemQueryFolderModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region NewQueryDefinition Command

        public RelayCommand<WorkItemQueryFolderModel> NewQueryDefinitionCommand { get; private set; }

        private void NewQueryDefinition(WorkItemQueryFolderModel workItemQueryDefinitionModel)
        {
            QueryFolder queryFolder = null;
            if (workItemQueryDefinitionModel != null)
            {
                queryFolder = workItemQueryDefinitionModel.QueryFolder;
            }

            TeamPilgrimPackage.TeamPilgrimVsService.NewQueryDefinition(_project, queryFolder);
        }

        private bool CanNewQueryDefinition(WorkItemQueryFolderModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region OpenQueryDefinition Command

        public RelayCommand<WorkItemQueryDefinitionModel> OpenQueryDefinitionCommand { get; private set; }

        private void OpenQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.OpenQueryDefinition(_projectCollection, workItemQueryDefinitionModel.QueryDefinition.Id);
        }

        private bool CanOpenQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region EditQueryDefinition Command

        public RelayCommand<WorkItemQueryDefinitionModel> EditQueryDefinitionCommand { get; private set; }

        private void EditQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.EditQueryDefinition(_projectCollection, workItemQueryDefinitionModel.QueryDefinition.Id);
        }

        private bool CanEditQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region DeleteQueryDefinition Command

        public RelayCommand<WorkItemQueryChildModel> DeleteQueryItemCommand { get; private set; }

        private void DeleteQueryDefinition(WorkItemQueryChildModel workItemQueryDefinitionModel)
        {
            bool result;

            var queryId = workItemQueryDefinitionModel.Id;

            if (_pilgrimServiceModelProvider.TryDeleteQueryItem(out result, _projectCollection, _project, queryId))
            {
                if (result)
                {
                    TeamPilgrimPackage.TeamPilgrimVsService.CloseQueryDefinitionFrames(_projectCollection, queryId);
                    workItemQueryDefinitionModel.ParentQueryFolder.QueryItems.Remove(workItemQueryDefinitionModel);
                }
            }
        }

        private bool CanDeleteQueryDefinition(WorkItemQueryChildModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion
    }
}