using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkspaceModel : BaseModel
    {
        public ObservableCollection<WorkItemModel> WorkItems { get; private set; }

        public ObservableCollection<PendingChangeModel> PendingChanges { get; private set; }

        public Workspace Workspace { get; private set; }

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

        private CheckinEvaluationResult _checkinEvaluationResult;
        public CheckinEvaluationResult CheckinEvaluationResult
        {
            get
            {
                return _checkinEvaluationResult;
            }
            private set
            {
                if (_checkinEvaluationResult == value) return;

                _checkinEvaluationResult = value;

                SendPropertyChanged("CheckinEvaluationResult");
            }
        }

        private WorkItemQueryDefinitionModel _selectedWorkWorkItemQueryDefinition;
        private ProjectCollectionModel _projectCollectionModel;

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

        public WorkspaceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionModel projectCollectionModel, Workspace workspace)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _projectCollectionModel = projectCollectionModel;
            Workspace = workspace;

            CheckInCommand = new RelayCommand(CheckIn, CanCheckIn);
            RefreshPendingChangesCommand = new RelayCommand(RefreshPendingChanges, CanRefreshPendingChanges);
            RefreshSelectedDefinitionWorkItemsCommand = new RelayCommand(RefreshSelectedDefinitionWorkItems, CanRefreshSelectedDefinitionWorkItems);
            ShowSelectWorkItemQueryCommand = new RelayCommand(ShowSelectWorkItemQuery, CanShowSelectWorkItemQuery);

            PendingChanges = new ObservableCollection<PendingChangeModel>();
            WorkItems = new ObservableCollection<WorkItemModel>();

            PendingChange[] pendingChanges;
            if (teamPilgrimServiceModelProvider.TryGetPendingChanges(out pendingChanges, Workspace))
            {
                foreach (var pendingChange in pendingChanges)
                {
                    var pendingChangeModel = new PendingChangeModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, pendingChange);
                    PendingChanges.Add(pendingChangeModel);
                }
            }

        }

        #region CheckIn Command

        public RelayCommand CheckInCommand { get; private set; }

        private void CheckIn()
        {
            var pendingChanges = PendingChanges
                .Where(model => model.IncludeChange)
                .Select(model => model.Change)
                .ToArray();

            var workItemChanges =
                WorkItems.Where(model => model.IsSelected)
                .Select(model => new WorkItemCheckinInfo(model.WorkItem, model.WorkItemCheckinAction.ToWorkItemCheckinAction())).ToArray();

            CheckinEvaluationResult checkinEvaluationResult;
            if (teamPilgrimServiceModelProvider.TryEvaluateCheckin(out checkinEvaluationResult, Workspace, pendingChanges, Comment, workItemChanges: workItemChanges))
            {
                if (checkinEvaluationResult.IsValid())
                {
                    CheckinEvaluationResult = null;
                    if (teamPilgrimServiceModelProvider.TryWorkspaceCheckin(Workspace, pendingChanges, Comment, workItemChanges: workItemChanges))
                    {
                        Comment = string.Empty;
                        RefreshPendingChanges();
                    }   
                }
                else
                {
                    CheckinEvaluationResult = checkinEvaluationResult;
                }
            }
        }

        private bool CanCheckIn()
        {
            return true;
        }

        #endregion

        #region RefreshPendingChanges Command

        public RelayCommand RefreshPendingChangesCommand { get; private set; }

        private void RefreshPendingChanges()
        {
            PendingChange[] currentPendingChanges;
            if (teamPilgrimServiceModelProvider.TryGetPendingChanges(out currentPendingChanges, Workspace))
            {
                var modelIntersection =
                    PendingChanges
                    .Join(currentPendingChanges, model => model.Change.PendingChangeId, change => change.PendingChangeId, (model, change) => model)
                    .ToArray();

                var modelsToRemove = PendingChanges.Where(model => !modelIntersection.Contains(model)).ToArray();

                var modelsToAdd = currentPendingChanges
                    .Where(pendingChange => !modelIntersection.Select(model => model.Change.PendingChangeId).Contains(pendingChange.PendingChangeId))
                    .Select(change => new PendingChangeModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, change)).ToArray();

                foreach (var modelToAdd in modelsToAdd)
                {
                    PendingChanges.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    PendingChanges.Remove(modelToRemove);
                }
            }
        }

        private bool CanRefreshPendingChanges()
        {
            return true;
        }

        #endregion

        #region RefreshSelectedDefinitionWorkItemsCommand Command

        public RelayCommand RefreshSelectedDefinitionWorkItemsCommand { get; private set; }

        private void RefreshSelectedDefinitionWorkItems()
        {
            WorkItemCollection workItemCollection;
            if (teamPilgrimServiceModelProvider.TryGetQueryDefinitionWorkItemCollection(out workItemCollection,
                                                                                        _projectCollectionModel.TfsTeamProjectCollection,
                                                                                        SelectedWorkItemQueryDefinition.QueryDefinition, SelectedWorkItemQueryDefinition.Project.Name))
            {
                var currentWorkItems = workItemCollection.Cast<WorkItem>().ToArray();

                var modelIntersection =
                    WorkItems
                    .Join(currentWorkItems, model => model.WorkItem.Id, workItem => workItem.Id, (model, change) => model)
                    .ToArray();

                var modelsToRemove = WorkItems.Where(model => !modelIntersection.Contains(model)).ToArray();

                var modelsToAdd = currentWorkItems
                        .Where(workItem => !modelIntersection.Select(workItemModel => workItemModel.WorkItem.Id).Contains(workItem.Id))
                        .Select(workItem => new WorkItemModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, workItem)).ToArray();

                foreach (var modelToAdd in modelsToAdd)
                {
                    WorkItems.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    WorkItems.Remove(modelToRemove);
                }
            }
        }

        private bool CanRefreshSelectedDefinitionWorkItems()
        {
            return true;
        }

        #endregion

        #region ShowSelectWorkItemQuery Command

        public RelayCommand ShowSelectWorkItemQueryCommand { get; private set; }

        private void ShowSelectWorkItemQuery()
        {
            var selectWorkItemQueryModel = new SelectWorkItemQueryModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, _projectCollectionModel);
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
    }
}