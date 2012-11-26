using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ShelveChanges
{
    public class ShelvesetServiceModel : BaseServiceModel
    {
        private readonly ProjectCollectionServiceModel _projectCollectionServiceModel;
        private readonly WorkspaceServiceModel _workspaceServiceModel;

        private string _shelvesetName;
        public string ShelvesetName
        {
            get
            {
                return _shelvesetName;
            }
            private set
            {
                if (_shelvesetName == value) return;

                _shelvesetName = value;

                SendPropertyChanged("ShelvesetName");
            }
        }

        private string _comment;
        public string Comment
        {
            get
            {
                return _comment;
            }
            private set
            {
                if (_comment == value) return;

                _comment = value;

                SendPropertyChanged("Comment");
            }
        }

        private WorkItemQueryDefinitionModel _selectedWorkWorkItemQueryDefinition;
        public WorkItemQueryDefinitionModel SelectedWorkItemQueryDefinition
        {
            get
            {
                return _selectedWorkWorkItemQueryDefinition;
            }
            private set
            {
                if (_selectedWorkWorkItemQueryDefinition == value) return;

                _selectedWorkWorkItemQueryDefinition = value;

                SendPropertyChanged("SelectedWorkItemQueryDefinition");

                RefreshSelectedDefinitionWorkItems();
            }
        }

        public ShelvesetServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider,
                                     ITeamPilgrimVsService teamPilgrimVsService,
                                     ProjectCollectionServiceModel projectCollectionServiceModel, WorkspaceServiceModel workspaceServiceModel)

            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _projectCollectionServiceModel = projectCollectionServiceModel;
            _workspaceServiceModel = workspaceServiceModel;

            ShowSelectWorkItemQueryCommand = new RelayCommand(ShowSelectWorkItemQuery, CanShowSelectWorkItemQuery);
            RefreshSelectedDefinitionWorkItemsCommand = new RelayCommand(RefreshSelectedDefinitionWorkItems, CanRefreshSelectedDefinitionWorkItems);

            SelectPendingChangesCommand = new RelayCommand<SelectPendingChangesCommandArgument>(SelectPendingChanges, CanSelectPendingChanges);
            SelectWorkItemsCommand = new RelayCommand<SelectWorkItemsCommandArgument>(SelectWorkItems, CanSelectWorkItems);

            CompareWithLatestCommand = new RelayCommand<ObservableCollection<object>>(CompareWithLatest, CanCompareWithLatest);
            CompareWithWorkspaceCommand = new RelayCommand<ObservableCollection<object>>(CompareWithWorkspace, CanCompareWithWorkspace);
            UndoPendingChangeCommand = new RelayCommand<ObservableCollection<object>>(UndoPendingChange, CanUndoPendingChange);
            PendingChangePropertiesCommand = new RelayCommand<ObservableCollection<object>>(PendingChangeProperties, CanPendingChangeProperties);

            PendingChanges = new TrulyObservableCollection<PendingChangeModel>(workspaceServiceModel.PendingChanges.Select(model => new PendingChangeModel(model.Change)
                {
                    IncludeChange = model.IncludeChange
                }).ToList());
            PendingChanges.CollectionChanged += PendingChangesOnCollectionChanged;

            WorkItems = new TrulyObservableCollection<WorkItemModel>();
            WorkItems.CollectionChanged += WorkItemsOnCollectionChanged;
        }

        #region PendingChanges Collection

        public TrulyObservableCollection<PendingChangeModel> PendingChanges { get; private set; }

        private void PendingChangesOnCollectionChanged(object sender,
                                                       NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
        }

        #endregion

        #region WorkItems Collection

        public TrulyObservableCollection<WorkItemModel> WorkItems { get; private set; }

        private void WorkItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        #endregion

        #region SelectPendingChanges Command

        public RelayCommand<SelectPendingChangesCommandArgument> SelectPendingChangesCommand { get; private set; }

        private void SelectPendingChanges(SelectPendingChangesCommandArgument selectPendingChangesCommandArgument)
        {
            foreach (var pendingChangeModel in selectPendingChangesCommandArgument.Collection)
            {
                pendingChangeModel.IncludeChange = selectPendingChangesCommandArgument.Value;
            }
        }

        private bool CanSelectPendingChanges(SelectPendingChangesCommandArgument collection)
        {
            return true;
        }

        #endregion

        #region SelectWorkItems Command

        public RelayCommand<SelectWorkItemsCommandArgument> SelectWorkItemsCommand { get; private set; }

        private void SelectWorkItems(SelectWorkItemsCommandArgument selectWorkItemsCommandArgument)
        {
            foreach (var workItemModel in selectWorkItemsCommandArgument.Collection)
            {
                workItemModel.IsSelected = selectWorkItemsCommandArgument.Value;
            }
        }

        private bool CanSelectWorkItems(SelectWorkItemsCommandArgument collection)
        {
            return true;
        }

        #endregion

        #region CompareWithWorkspace Command

        public RelayCommand<ObservableCollection<object>> CompareWithWorkspaceCommand { get; private set; }

        private void CompareWithWorkspace(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.CompareChangesetChangesWithWorkspaceVersions(_workspaceServiceModel.Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanCompareWithWorkspace(ObservableCollection<object> collection)
        {
            return collection.Count == 1;
        }

        #endregion

        #region CompareWithLatest Command

        public RelayCommand<ObservableCollection<object>> CompareWithLatestCommand { get; private set; }

        private void CompareWithLatest(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.CompareChangesetChangesWithLatestVersions(_workspaceServiceModel.Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanCompareWithLatest(ObservableCollection<object> collection)
        {
            return collection.Count == 1;
        }

        #endregion

        #region UndoPendingChange Command

        public RelayCommand<ObservableCollection<object>> UndoPendingChangeCommand { get; private set; }

        private void UndoPendingChange(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.UndoChanges(_workspaceServiceModel.Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanUndoPendingChange(ObservableCollection<object> collection)
        {
            return true;
        }

        #endregion

        #region PendingChangeProperties Command

        public RelayCommand<ObservableCollection<object>> PendingChangePropertiesCommand { get; private set; }

        private void PendingChangeProperties(ObservableCollection<object> collection)
        {
        }

        private bool CanPendingChangeProperties(ObservableCollection<object> collection)
        {
            return true;
        }

        #endregion

        #region ShowSelectWorkItemQuery Command

        public RelayCommand ShowSelectWorkItemQueryCommand { get; private set; }

        private void ShowSelectWorkItemQuery()
        {
            var selectWorkItemQueryModel = new SelectWorkItemQueryModel(_projectCollectionServiceModel);
            var selectWorkItemQueryDialog = new SelectWorkItemQueryDialog
                {
                    DataContext = selectWorkItemQueryModel
                };

            var dialogResult = selectWorkItemQueryDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                SelectedWorkItemQueryDefinition = selectWorkItemQueryModel.SelectedWorkItemQueryDefinition;
            }
        }

        private bool CanShowSelectWorkItemQuery()
        {
            return true;
        }

        #endregion

        #region RefreshSelectedDefinitionWorkItemsCommand Command

        public RelayCommand RefreshSelectedDefinitionWorkItemsCommand { get; private set; }

        private void RefreshSelectedDefinitionWorkItems()
        {
            if (SelectedWorkItemQueryDefinition == null)
                return;

            WorkItemCollection workItemCollection;
            if (teamPilgrimServiceModelProvider.TryGetQueryDefinitionWorkItemCollection(out workItemCollection,
                                                                                        _projectCollectionServiceModel.TfsTeamProjectCollection,
                                                                                        SelectedWorkItemQueryDefinition.QueryDefinition,
                                                                                        SelectedWorkItemQueryDefinition.Project.Name))
            {
                var currentWorkItems = workItemCollection.Cast<WorkItem>().ToArray();

                var modelIntersection =
                    WorkItems
                        .Join(currentWorkItems, model => model.WorkItem.Id, workItem => workItem.Id,
                              (model, change) => model)
                        .ToArray();

                var modelsToRemove = WorkItems.Where(model => !modelIntersection.Contains(model)).ToArray();

                var modelsToAdd = currentWorkItems
                    .Where(workItem => !modelIntersection.Select(workItemModel => workItemModel.WorkItem.Id).Contains(workItem.Id))
                    .Select(workItem => new WorkItemModel(workItem)).ToArray();

                foreach (var modelToAdd in modelsToAdd)
                {
                    WorkItems.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    WorkItems.Remove(modelToRemove);
                }

                var selectedWorkItemCheckinActionEnum = TeamPilgrimPackage.TeamPilgrimSettings.SelectedWorkItemCheckinAction;
                foreach (var workItemModel in WorkItems.Where(model => !model.IsSelected))
                {
                    workItemModel.WorkItemCheckinAction = selectedWorkItemCheckinActionEnum;
                }
            }
        }

        private bool CanRefreshSelectedDefinitionWorkItems()
        {
            return true;
        }

        #endregion
    }
}
