using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkspaceServiceModel : BaseServiceModel, IPendingChangeCommandModel
    {
        public ObservableCollection<WorkItemModel> WorkItems { get; private set; }

        public ObservableCollection<PendingChangeModel> PendingChanges { get; private set; }

        public ObservableCollection<CheckinNoteModel> CheckinNotes { get; private set; }

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

                var previousValue = _comment;

                _comment = value;

                SendPropertyChanged("Comment");

                if (string.IsNullOrWhiteSpace(previousValue) ^ string.IsNullOrWhiteSpace(_comment))
                {
                    EvaluateCheckIn();
                }
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

        private readonly ProjectCollectionServiceModel _projectCollectionServiceModel;
        private readonly CheckinNotesCacheWrapper _checkinNotesCacheWrapper;

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

        public WorkspaceServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionServiceModel projectCollectionServiceModel, Workspace workspace)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _projectCollectionServiceModel = projectCollectionServiceModel;
            Workspace = workspace;

            CheckInCommand = new RelayCommand(CheckIn, CanCheckIn);
            RefreshPendingChangesCommand = new RelayCommand(RefreshPendingChanges, CanRefreshPendingChanges);
            RefreshSelectedDefinitionWorkItemsCommand = new RelayCommand(RefreshSelectedDefinitionWorkItems, CanRefreshSelectedDefinitionWorkItems);
            ShowSelectWorkItemQueryCommand = new RelayCommand(ShowSelectWorkItemQuery, CanShowSelectWorkItemQuery);
            EvaluateCheckInCommand = new RelayCommand(EvaluateCheckIn, CanEvaluateCheckIn);
            ViewPendingChangeCommand = new RelayCommand<PendingChangeModel>(ViewPendingChange, CanViewPendingChange);

            PendingChanges = new ObservableCollection<PendingChangeModel>();
            WorkItems = new ObservableCollection<WorkItemModel>();
            CheckinNotes = new ObservableCollection<CheckinNoteModel>();

            PendingChange[] pendingChanges;
            if (teamPilgrimServiceModelProvider.TryGetPendingChanges(out pendingChanges, Workspace))
            {
                foreach (var pendingChange in pendingChanges)
                {
                    var pendingChangeModel = new PendingChangeModel(this, pendingChange);
                    PendingChanges.Add(pendingChangeModel);
                }
            }

            var versionControlServer = _projectCollectionServiceModel.TfsTeamProjectCollection.GetService<VersionControlServer>();
            versionControlServer.PendingChangesChanged += VersionControlServerOnPendingChangesChanged;

            _checkinNotesCacheWrapper = new CheckinNotesCacheWrapper(versionControlServer);
        }

        private void VersionControlServerOnPendingChangesChanged(object sender, WorkspaceEventArgs workspaceEventArgs)
        {
            RefreshPendingChanges();
        }

        #region ViewPendingChange Command

        public RelayCommand<PendingChangeModel> ViewPendingChangeCommand { get; private set; }

        private void ViewPendingChange(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanViewPendingChange(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

        #region CompareWithUnmodified Command

        public RelayCommand<PendingChangeModel> CompareWithUnmodifiedCommand { get; private set; }

        private void CompareWithUnmodified(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanCompareWithUnmodified(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

        #region CompareWithWorkspace Command

        public RelayCommand<PendingChangeModel> CompareWithWorkspaceCommand { get; private set; }

        private void CompareWithWorkspace(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanCompareWithWorkspaceChange(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

        #region CompareWithLatest Command

        public RelayCommand<PendingChangeModel> CompareWithLatestCommand { get; private set; }

        private void CompareWithLatest(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanCompareWithLatest(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

        #region UndoPendingChange Command

        public RelayCommand<PendingChangeModel> UndoPendingChangeCommand { get; private set; }

        private void UndoPendingChange(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanUndoPendingChange(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

        #region RefreshPendingChange Command

        public RelayCommand<PendingChangeModel> RefreshPendingChangeCommand { get; private set; }

        private void RefreshPendingChange(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanRefreshPendingChange(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

        #region PendingChangeProperties Command

        public RelayCommand<PendingChangeModel> PendingChangePropertiesCommand { get; private set; }

        private void PendingChangeProperties(PendingChangeModel pendingChangeModel)
        {
        }

        private bool CanPendingChangeProperties(PendingChangeModel pendingChangeModel)
        {
            return true;
        }

        #endregion

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

            var missingCheckinNotes = CheckinNotes
                .Where(model => model.CheckinNoteFieldDefinition.Required && string.IsNullOrWhiteSpace(model.Value))
                .Select(model => model.CheckinNoteFieldDefinition.Name).ToArray();

            if (missingCheckinNotes.Any())
            {
                Messenger.Default.Send(new ShowPendingChangesTabItemMessage { ShowPendingChangesTabItem = ShowPendingChangesTabItemEnum.CheckinNotes });

                MessageBox.Show("Check-in Validation\r\n\r\nEnter a value for " + string.Join(", ", missingCheckinNotes), "Team Pilgrim", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var checkinNoteFieldValues =
                CheckinNotes
                .Where(model => !string.IsNullOrWhiteSpace(model.Value))
                .Select(model => new CheckinNoteFieldValue(model.CheckinNoteFieldDefinition.Name, model.Value))
                .ToArray();

            var checkinNote = new CheckinNote(checkinNoteFieldValues);

            CheckinEvaluationResult checkinEvaluationResult;
            if (teamPilgrimServiceModelProvider.TryEvaluateCheckin(out checkinEvaluationResult, Workspace, pendingChanges, Comment, checkinNote, workItemChanges))
            {
                if (checkinEvaluationResult.IsValid())
                {
                    ProcessCheckIn(pendingChanges, checkinNote, workItemChanges);
                }
                else if (checkinEvaluationResult.Conflicts.Any())
                {
                    MessageBox.Show(
                        "Check In\r\n\r\nNo files checked in due to conflicting changes. Please use Conflicts Manager to resolve conflicts and try again.", "Team Pilgrim", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    var conflictedServerItems = checkinEvaluationResult.Conflicts.Select(conflict => conflict.ServerItem).ToArray();
                    teamPilgrimVsService.ResolveConflicts(Workspace, conflictedServerItems, false, false);
                }
                else if (checkinEvaluationResult.PolicyFailures.Any())
                {
                    Messenger.Default.Send(new ShowPendingChangesTabItemMessage { ShowPendingChangesTabItem = ShowPendingChangesTabItemEnum.PolicyWarnings });

                    var policyFailureModel = new PolicyFailureModel();
                    var policyFailureDialog = new PolicyFailureDialog()
                    {
                        DataContext = policyFailureModel
                    };

                    var dialogResult = policyFailureDialog.ShowDialog();
                    if (dialogResult.HasValue && dialogResult.Value && policyFailureModel.Override)
                    {
                        string reason = policyFailureModel.Reason;
                        ProcessCheckIn(pendingChanges, checkinNote, workItemChanges, new PolicyOverrideInfo(reason, checkinEvaluationResult.PolicyFailures));
                    }
                    else
                    {
                        CheckinEvaluationResult = checkinEvaluationResult;
                    }
                }
            }
        }

        private void ProcessCheckIn(PendingChange[] pendingChanges, CheckinNote checkinNote, WorkItemCheckinInfo[] workItemChanges, PolicyOverrideInfo policyOverrideInfo = null)
        {
            CheckinEvaluationResult = null;
            if (teamPilgrimServiceModelProvider.TryWorkspaceCheckin(Workspace, pendingChanges, Comment, checkinNote, workItemChanges, policyOverrideInfo))
            {
                Comment = string.Empty;
                RefreshPendingChanges();

                foreach (var workItem in WorkItems.Where(model => model.IsSelected))
                {
                    workItem.IsSelected = false;
                }

                RefreshSelectedDefinitionWorkItemsCommand.Execute(null);
            }
        }

        private bool CanCheckIn()
        {
            return true;
        }

        #endregion

        #region EvaluateCheckIn Command

        public RelayCommand EvaluateCheckInCommand { get; private set; }

        private void EvaluateCheckIn()
        {
            var pendingChanges = PendingChanges
                                    .Where(model => model.IncludeChange)
                                    .Select(model => model.Change)
                                    .ToArray();

            var currentCheckinNoteDefinitions = _checkinNotesCacheWrapper.GetCheckinNotes(pendingChanges);

            var equalityComparer = CheckinNoteFieldDefinition.NameComparer.ToGenericComparer<CheckinNoteFieldDefinition>().ToEqualityComparer();

            var modelIntersection =
                    CheckinNotes
                    .Join(currentCheckinNoteDefinitions, model => model.CheckinNoteFieldDefinition, checkinNoteFieldDefinition => checkinNoteFieldDefinition, (model, change) => model, equalityComparer)
                    .ToArray();

            var modelsToRemove = CheckinNotes.Where(model => !modelIntersection.Contains(model)).ToArray();

            var modelsToAdd = currentCheckinNoteDefinitions
                .Where(checkinNoteFieldDefinition => !modelIntersection.Select(model => model.CheckinNoteFieldDefinition).Contains(checkinNoteFieldDefinition, equalityComparer))
                .Select(checkinNoteFieldDefinition => new CheckinNoteModel(checkinNoteFieldDefinition)).ToArray();

            foreach (var modelToAdd in modelsToAdd)
            {
                CheckinNotes.Add(modelToAdd);
            }

            foreach (var modelToRemove in modelsToRemove)
            {
                CheckinNotes.Remove(modelToRemove);
            }

            CheckinEvaluationResult checkinEvaluationResult;

            var workItemChanges =
                WorkItems
                .Where(model => model.IsSelected)
                .Select(model => new WorkItemCheckinInfo(model.WorkItem, model.WorkItemCheckinAction.ToWorkItemCheckinAction())).ToArray();

            var checkinNoteFieldValues =
                CheckinNotes
                .Where(model => !string.IsNullOrWhiteSpace(model.Value))
                .Select(model => new CheckinNoteFieldValue(model.CheckinNoteFieldDefinition.Name, model.Value))
                .ToArray();

            var checkinNote = new CheckinNote(checkinNoteFieldValues);

            if (teamPilgrimServiceModelProvider.TryEvaluateCheckin(out checkinEvaluationResult, Workspace, pendingChanges, Comment, checkinNote, workItemChanges))
            {
                CheckinEvaluationResult = checkinEvaluationResult;
            }
        }

        private bool CanEvaluateCheckIn()
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
                    .Select(change => new PendingChangeModel(this, change)).ToArray();

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
            if (SelectedWorkItemQueryDefinition == null)
                return;

            WorkItemCollection workItemCollection;
            if (teamPilgrimServiceModelProvider.TryGetQueryDefinitionWorkItemCollection(out workItemCollection,
                                                                                        _projectCollectionServiceModel
                                                                                            .
                                                                                            TfsTeamProjectCollection,
                                                                                        SelectedWorkItemQueryDefinition
                                                                                            .QueryDefinition,
                                                                                        SelectedWorkItemQueryDefinition
                                                                                            .Project.Name))
            {
                var currentWorkItems = workItemCollection.Cast<WorkItem>().ToArray();

                var modelIntersection =
                    WorkItems
                        .Join(currentWorkItems, model => model.WorkItem.Id, workItem => workItem.Id,
                              (model, change) => model)
                        .ToArray();

                var modelsToRemove = WorkItems.Where(model => !modelIntersection.Contains(model)).ToArray();

                var modelsToAdd = currentWorkItems
                    .Where(
                        workItem =>
                        !modelIntersection.Select(workItemModel => workItemModel.WorkItem.Id).Contains(workItem.Id))
                    .Select(workItem => new WorkItemModel(workItem)).ToArray();

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
    }
}