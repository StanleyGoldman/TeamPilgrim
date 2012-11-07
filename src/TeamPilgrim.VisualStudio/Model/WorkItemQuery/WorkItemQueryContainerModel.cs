using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public class WorkItemQueryContainerModel : BaseModel, IWorkItemQueryCommandModel
    {
        private readonly ITeamPilgrimServiceModelProvider _teamPilgrimServiceModelProvider;

        private readonly TeamPilgrimModel _teamPilgrimModel;

        private readonly TfsTeamProjectCollection _projectCollection;

        private readonly Project _project;

        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public WorkItemQueryContainerModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimModel teamPilgrimModel, TfsTeamProjectCollection projectCollection, Project project)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _teamPilgrimServiceModelProvider = teamPilgrimServiceModelProvider;
            _teamPilgrimModel = teamPilgrimModel;
            _projectCollection = projectCollection;
            _project = project;

            NewWorkItemCommand = new RelayCommand<string>(NewWorkItem, CanNewWorkItem);
            GoToWorkItemCommand = new RelayCommand(GoToWorkItem, CanGoToWorkItem);

            NewQueryDefinitionCommand = new RelayCommand<WorkItemQueryFolderModel>(NewQueryDefinition, CanNewQueryDefinition);
            NewQueryFolderCommand = new RelayCommand<WorkItemQueryFolderModel>(NewQueryFolder, CanNewQueryFolder);

            OpenQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(OpenQueryDefinition, CanOpenQueryDefinition);
            EditQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(EditQueryDefinition, CanEditQueryDefinition);

            DeleteQueryItemCommand = new RelayCommand<WorkItemQueryChildModel>(DeleteQueryDefinition, CanDeleteQueryDefinition);
            OpenSeurityDialogCommand = new RelayCommand<WorkItemQueryChildModel>(OpenSeurityDialog, CanOpenSeurityDialog);

            var queryHierarchy = _project.QueryHierarchy;
            var queryItemModels = queryHierarchy.GetQueryItemViewModels(this, teamPilgrimServiceModelProvider, teamPilgrimVsService, project, 1);

            QueryItems = new ObservableCollection<WorkItemQueryChildModel>(queryItemModels);
        }

        #region GoToWorkItem Command

        public RelayCommand GoToWorkItemCommand { get; private set; }

        private void GoToWorkItem()
        {
            teamPilgrimVsService.GoToWorkItem();
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
            teamPilgrimVsService.NewWorkItem(_projectCollection, _project.Name, workItemType);
        }

        private bool CanNewWorkItem(string workItemType)
        {
            return true;
        }

        #endregion

        #region NewQueryFolder Command

        public RelayCommand<WorkItemQueryFolderModel> NewQueryFolderCommand { get; private set; }

        private void NewQueryFolder(WorkItemQueryFolderModel workItemQueryFolderModel)
        {
            QueryFolder queryFolder;
            QueryFolder parentFolder = workItemQueryFolderModel.QueryFolder;
            if (_teamPilgrimServiceModelProvider.TryAddNewQueryFolder(out queryFolder, _projectCollection, _project, parentFolder.Id))
            {
                workItemQueryFolderModel.QueryItems.Insert(0, new WorkItemQueryFolderModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, this, _project, workItemQueryFolderModel.Depth + 1, queryFolder, new WorkItemQueryChildModel[0]));
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

            teamPilgrimVsService.NewQueryDefinition(_project, queryFolder);
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
            teamPilgrimVsService.OpenQueryDefinition(_projectCollection, workItemQueryDefinitionModel.QueryDefinition.Id);
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
            teamPilgrimVsService.EditQueryDefinition(_projectCollection, workItemQueryDefinitionModel.QueryDefinition.Id);
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

            if (_teamPilgrimServiceModelProvider.TryDeleteQueryItem(out result, _projectCollection, _project, queryId))
            {
                if (result)
                {
                    teamPilgrimVsService.CloseQueryDefinitionFrames(_projectCollection, queryId);
                    workItemQueryDefinitionModel.ParentQueryFolder.QueryItems.Remove(workItemQueryDefinitionModel);
                }
            }
        }

        private bool CanDeleteQueryDefinition(WorkItemQueryChildModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region OpenSeurityDialog Command

        public RelayCommand<WorkItemQueryChildModel> OpenSeurityDialogCommand { get; private set; }

        private void OpenSeurityDialog(WorkItemQueryChildModel workItemQueryDefinitionModel)
        {
            QueryItem queryItem = null;

            var itemQueryDefinitionModel = workItemQueryDefinitionModel as WorkItemQueryDefinitionModel;
            if (itemQueryDefinitionModel != null)
            {
                queryItem = itemQueryDefinitionModel.QueryDefinition;
            }

            var itemQueryFolderModel = workItemQueryDefinitionModel as WorkItemQueryFolderModel;
            if (itemQueryFolderModel != null)
            {
                queryItem = itemQueryFolderModel.QueryFolder;
            }

            Debug.Assert(queryItem != null, "queryItem != null");
            teamPilgrimVsService.OpenSecurityItemDialog(queryItem);
        }

        private bool CanOpenSeurityDialog(WorkItemQueryChildModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion
    }
}