using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItems;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public class WorkItemQueryServiceModel : BaseServiceModel, IWorkItemQueryCommandModel
    {
        private readonly TfsTeamProjectCollection _projectCollection;

        private readonly Project _project;
        
        private readonly BackgroundWorker _populateBackgroundWorker;

        private bool _isPopulating;
        public bool IsPopulating
        {
            get
            {
                return _isPopulating;
            }
            set
            {
                if (_isPopulating == value) return;

                _isPopulating = value;

                SendPropertyChanged("IsPopulating");
            }
        }

        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public WorkItemQueryServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TfsTeamProjectCollection projectCollection, Project project)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _projectCollection = projectCollection;
            _project = project;

            QueryItems = new ObservableCollection<WorkItemQueryChildModel>();

            NewWorkItemCommand = new RelayCommand<string>(NewWorkItem, CanNewWorkItem);

            GoToWorkItemCommand = new RelayCommand(GoToWorkItem, CanGoToWorkItem);

            NewQueryDefinitionCommand = new RelayCommand<WorkItemQueryFolderModel>(NewQueryDefinition, CanNewQueryDefinition);
            NewQueryFolderCommand = new RelayCommand<WorkItemQueryFolderModel>(NewQueryFolder, CanNewQueryFolder);

            OpenQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(OpenQueryDefinition, CanOpenQueryDefinition);
            EditQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(EditQueryDefinition, CanEditQueryDefinition);

            DeleteQueryItemCommand = new RelayCommand<WorkItemQueryChildModel>(DeleteQueryDefinition, CanDeleteQueryDefinition);
            OpenSeurityDialogCommand = new RelayCommand<WorkItemQueryChildModel>(OpenSeurityDialog, CanOpenSeurityDialog);

            _populateBackgroundWorker = new BackgroundWorker();
            _populateBackgroundWorker.DoWork +=PopulateBackgroundWorkerOnDoWork;
            _populateBackgroundWorker.RunWorkerAsync();
        }

        private void PopulateBackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            this.Logger().Trace("Begin Populate");

            Application.Current.Dispatcher.Invoke(() =>
                {
                    IsPopulating = true;
                    QueryItems.Clear();
                }, DispatcherPriority.Normal);

            var queryItemModels = _project.QueryHierarchy.GetQueryItemViewModels(this,
                                                                                 teamPilgrimServiceModelProvider,
                                                                                 teamPilgrimVsService, _project,
                                                                                 1);
            foreach (var queryChildModel in queryItemModels)
            {
                var localScopeModel = queryChildModel;
                Application.Current.Dispatcher.Invoke(() => QueryItems.Add(localScopeModel));
            }

            Application.Current.Dispatcher.Invoke(() => IsPopulating = false, DispatcherPriority.Normal);

            this.Logger().Trace("End Populate");
        }

        #region Refresh Command

        protected override void Refresh()
        {
            _populateBackgroundWorker.RunWorkerAsync();
        }

        protected override bool CanRefresh()
        {
            return true;
        }

        #endregion

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
            if (teamPilgrimServiceModelProvider.TryAddNewQueryFolder(out queryFolder, _projectCollection, _project, parentFolder.Id))
            {
                workItemQueryFolderModel.QueryItems.Insert(0, new WorkItemQueryFolderModel(this, _project, workItemQueryFolderModel.Depth + 1, queryFolder, new WorkItemQueryChildModel[0], null));
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
            WitDefaultCommandHandlerWrapper.ViewQuery(workItemQueryDefinitionModel.QueryDefinition);
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

            if (teamPilgrimServiceModelProvider.TryDeleteQueryItem(out result, _projectCollection, _project, queryId))
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