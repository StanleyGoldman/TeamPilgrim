using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItems;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ShelveChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NLog;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkspaceServiceModel : BaseServiceModel
    {
        public delegate void ShowShelveDialogDelegate(ShelvesetServiceModel shelvesetServiceModel);
        public event ShowShelveDialogDelegate ShowShelveDialog;

        public delegate void ShowUnshelveDialogDelegate();
        public event ShowUnshelveDialogDelegate ShowUnshelveDialog;

        public delegate void ShowPendingChangesItemDelegate(ShowPendingChangesTabItemEnum showPendingChangesTabItemEnum);
        public event ShowPendingChangesItemDelegate ShowPendingChangesItem;

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
                    this.Logger().Debug("Comment IsNullOrWhiteSpace Status Changed");
                    EvaluateCheckInCommand.Execute(null);
                }
            }
        }

        private PreviouslySelectedWorkItemQuery[] _previouslySelectedWorkItemQueries;
        public PreviouslySelectedWorkItemQuery[] PreviouslySelectedWorkItemQueries
        {
            get
            {
                return _previouslySelectedWorkItemQueries;
            }
            private set
            {
                if (_previouslySelectedWorkItemQueries == value) return;

                _previouslySelectedWorkItemQueries = value;

                SendPropertyChanged("PreviouslySelectedWorkItemQueries");
                SendPropertyChanged("CurrentPreviouslySelectedWorkItemQuery");
            }
        }

        public PreviouslySelectedWorkItemQuery CurrentPreviouslySelectedWorkItemQuery
        {
            get
            {
                if (SelectedWorkItemQueryDefinition == null)
                    return null;

                var current = PreviouslySelectedWorkItemQueries.FirstOrDefault(model => model.WorkItemQueryPath == SelectedWorkItemQueryDefinition.QueryDefinition.Path);

                return current;
            }
            set
            {
                if (value != null)
                {
                    var selectedWorkItemQueryDefinition = (WorkItemQueryDefinitionModel)_projectCollectionServiceModel.ProjectModels
                        .Select(model => model.WorkItemQueryServiceModel.QueryItems.FindWorkItemQueryChildModelMatchingPath(value.WorkItemQueryPath))
                        .FirstOrDefault(model => model != null);

                    if (selectedWorkItemQueryDefinition == null)
                    {
                        TeamPilgrimPackage.TeamPilgrimSettings.RemovePreviouslySelectedWorkItemQuery(_projectCollectionServiceModel.TfsTeamProjectCollection.Uri.ToString(), value.WorkItemQueryPath);

                        if (SelectedWorkItemQueryDefinition == null)
                        {
                            PopulatePreviouslySelectedWorkItemQueryModels();
                        }
                        else
                        {
                            SelectedWorkItemQueryDefinition = null;
                        }
                    }
                    else
                    {
                        SelectedWorkItemQueryDefinition = selectedWorkItemQueryDefinition;
                    }
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
        internal readonly CheckinNotesCacheWrapper checkinNotesCacheWrapper;

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

                TeamPilgrimPackage.TeamPilgrimSettings.AddPreviouslySelectedWorkItemQuery(_projectCollectionServiceModel.TfsTeamProjectCollection.Uri.ToString(), value.QueryDefinition.Path);
                PopulatePreviouslySelectedWorkItemQueryModels();
            }
        }

        private bool _solutionIsOpen;
        public bool SolutionIsOpen
        {
            get
            {
                return _solutionIsOpen;
            }
            private set
            {
                if (_solutionIsOpen == value) return;

                _solutionIsOpen = value;

                SendPropertyChanged("SolutionIsOpen");

                RefreshPendingChangesCommand.Execute(null);
            }
        }

        private bool _filterSolution;
        public bool FilterSolution
        {
            get
            {
                return _filterSolution;
            }
            set
            {
                if (_filterSolution == value) return;

                _filterSolution = value;

                SendPropertyChanged("FilterSolution");
                RefreshPendingChangesCommand.Execute(null);
            }
        }

        private CollectionSelectionSummaryEnum _pendingChangesSummary = CollectionSelectionSummaryEnum.None;
        public CollectionSelectionSummaryEnum PendingChangesSummary
        {
            get { return _pendingChangesSummary; }
            set
            {
                if (_pendingChangesSummary == value)
                    return;
                
                _pendingChangesSummary = value;

                SendPropertyChanged("PendingChangesSummary");
            }
        }

        private bool _backgroundFunctionPreventDataUpdate;

        public WorkspaceServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionServiceModel projectCollectionServiceModel, Workspace workspace)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _projectCollectionServiceModel = projectCollectionServiceModel;
            Workspace = workspace;

            var versionControlServer = _projectCollectionServiceModel.TfsTeamProjectCollection.GetService<VersionControlServer>();
            versionControlServer.PendingChangesChanged += VersionControlServerOnPendingChangesChanged;

            checkinNotesCacheWrapper = new CheckinNotesCacheWrapper(versionControlServer);

            ShelveCommand = new RelayCommand(Shelve, CanShelve);
            UnshelveCommand = new RelayCommand(Unshelve, CanUnshelve);
            CheckInCommand = new RelayCommand(CheckIn, CanCheckIn);
            RefreshPendingChangesCommand = new RelayCommand(RefreshPendingChanges, CanRefreshPendingChanges);
            RefreshSelectedDefinitionWorkItemsCommand = new RelayCommand(RefreshSelectedDefinitionWorkItems, CanRefreshSelectedDefinitionWorkItems);
            ShowSelectWorkItemQueryCommand = new RelayCommand(ShowSelectWorkItemQuery, CanShowSelectWorkItemQuery);
            EvaluateCheckInCommand = new RelayCommand(EvaluateCheckIn, CanEvaluateCheckIn);

            SelectPendingChangesCommand = new RelayCommand<SelectPendingChangesCommandArgument>(SelectPendingChanges, CanSelectPendingChanges);
            SelectWorkItemsCommand = new RelayCommand<SelectWorkItemsCommandArgument>(SelectWorkItems, CanSelectWorkItems);

            ViewWorkItemCommand = new RelayCommand<ObservableCollection<object>>(ViewWorkItem, CanViewWorkItem);
            ViewPendingChangeCommand = new RelayCommand<ObservableCollection<object>>(ViewPendingChange, CanViewPendingChange);
            CompareWithLatestCommand = new RelayCommand<ObservableCollection<object>>(CompareWithLatest, CanCompareWithLatest);
            CompareWithWorkspaceCommand = new RelayCommand<ObservableCollection<object>>(CompareWithWorkspace, CanCompareWithWorkspace);
            UndoPendingChangeCommand = new RelayCommand<ObservableCollection<object>>(UndoPendingChange, CanUndoPendingChange);
            PendingChangePropertiesCommand = new RelayCommand<ObservableCollection<object>>(PendingChangeProperties, CanPendingChangeProperties);

            CheckinNotes = new ObservableCollection<CheckinNoteModel>();

            PendingChanges = new TrulyObservableCollection<PendingChangeModel>();

            PendingChange[] pendingChanges;
            if (teamPilgrimServiceModelProvider.TryGetPendingChanges(out pendingChanges, Workspace))
            {
                foreach (var pendingChange in pendingChanges)
                {
                    var pendingChangeModel = new PendingChangeModel(pendingChange);
                    PendingChanges.Add(pendingChangeModel);
                }
            }
            PendingChanges.CollectionChanged += PendingChangesOnCollectionChanged;

            SolutionIsOpen = teamPilgrimVsService.Solution.IsOpen && !string.IsNullOrEmpty(teamPilgrimVsService.Solution.FileName);
            teamPilgrimVsService.SolutionStateChanged += () =>
            {
                SolutionIsOpen = teamPilgrimVsService.Solution.IsOpen && !string.IsNullOrEmpty(teamPilgrimVsService.Solution.FileName);
            };

            WorkItems = new TrulyObservableCollection<WorkItemModel>();
            WorkItems.CollectionChanged += WorkItemsOnCollectionChanged;

            PopulatePreviouslySelectedWorkItemQueryModels();
        }

        private void PopulatePreviouslySelectedWorkItemQueryModels()
        {
            var previouslySelectedWorkItemsQuery = TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemsQueries[_projectCollectionServiceModel.TfsTeamProjectCollection.Uri.ToString()];
            PreviouslySelectedWorkItemQueries = previouslySelectedWorkItemsQuery.Select(workItemQueryPath => new PreviouslySelectedWorkItemQuery(workItemQueryPath)).ToArray();
        }

        private void PopulateSelectedPendingChangesSummary()
        {
            if (_backgroundFunctionPreventDataUpdate)
                return;

            this.Logger().Trace("PopulateSelectedPendingChangesSummary");

            if (PendingChanges.Count == 0)
            {
                PendingChangesSummary = CollectionSelectionSummaryEnum.None;
                return;
            }

            var includedCount = PendingChanges.Count(model => model.IncludeChange);
            if (includedCount == 0)
            {
                PendingChangesSummary = CollectionSelectionSummaryEnum.None;
                return;
            }

            PendingChangesSummary = PendingChanges.Count == includedCount
                                        ? CollectionSelectionSummaryEnum.All
                                        : CollectionSelectionSummaryEnum.Some;
        }

        private void VersionControlServerOnPendingChangesChanged(object sender, WorkspaceEventArgs workspaceEventArgs)
        {
            this.Logger().Debug("VersionControlServerOnPendingChangesChanged");
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)RefreshPendingChanges);
        }

        protected virtual void OnShowPendingChangesItem(ShowPendingChangesTabItemEnum showpendingchangestabitemenum)
        {
            var handler = ShowPendingChangesItem;
            if (handler != null) handler(showpendingchangestabitemenum);
        }

        #region PendingChanges Collection

        public TrulyObservableCollection<PendingChangeModel> PendingChanges { get; private set; }

        private void PendingChangesOnCollectionChanged(object sender,
                                                       NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.Logger().Trace("PendingChangesOnCollectionChanged");
           
            PopulateSelectedPendingChangesSummary();
            EvaluateCheckInCommand.Execute(null);
        }

        #endregion

        #region WorkItems Collection

        public TrulyObservableCollection<WorkItemModel> WorkItems { get; private set; }

        private void WorkItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Logger().Trace("WorkItemsOnCollectionChanged");

            EvaluateCheckInCommand.Execute(null);
        }

        #endregion

        #region ViewPendingChange Command

        public RelayCommand<ObservableCollection<object>> ViewPendingChangeCommand { get; private set; }

        private void ViewPendingChange(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.View(Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanViewPendingChange(ObservableCollection<object> collection)
        {
            return collection != null && collection.Any();
        }

        #endregion

        #region SelectPendingChanges Command

        public RelayCommand<SelectPendingChangesCommandArgument> SelectPendingChangesCommand { get; private set; }

        private void SelectPendingChanges(SelectPendingChangesCommandArgument selectPendingChangesCommandArgument)
        {
            this.Logger().Debug("Select Pending Changes: {0}, Count: {1}", selectPendingChangesCommandArgument.Value, selectPendingChangesCommandArgument.Collection.Count());

            _backgroundFunctionPreventDataUpdate = true;

            foreach (var pendingChangeModel in selectPendingChangesCommandArgument.Collection)
            {
                pendingChangeModel.IncludeChange = selectPendingChangesCommandArgument.Value;
            }

            _backgroundFunctionPreventDataUpdate = false;

            PopulateSelectedPendingChangesSummary();
            EvaluateCheckInCommand.Execute(null);
        }

        private bool CanSelectPendingChanges(SelectPendingChangesCommandArgument commandArgument)
        {
            return commandArgument.Collection != null && commandArgument.Collection.Any();
        }

        #endregion

        #region SelectWorkItems Command

        public RelayCommand<SelectWorkItemsCommandArgument> SelectWorkItemsCommand { get; private set; }

        private void SelectWorkItems(SelectWorkItemsCommandArgument selectWorkItemsCommandArgument)
        {
            this.Logger().Debug("Select Work Items: {0}, Count: {1}", selectWorkItemsCommandArgument.Value, selectWorkItemsCommandArgument.Collection.Count());

            _backgroundFunctionPreventDataUpdate = true;

            foreach (var workItemModel in selectWorkItemsCommandArgument.Collection)
            {
                workItemModel.IsSelected = selectWorkItemsCommandArgument.Value;
            }

            _backgroundFunctionPreventDataUpdate = false;
            EvaluateCheckInCommand.Execute(null);
        }

        private bool CanSelectWorkItems(SelectWorkItemsCommandArgument commandArgument)
        {
            return commandArgument.Collection != null && commandArgument.Collection.Any();
        }

        #endregion

        #region ViewWorkItem Command

        public RelayCommand<ObservableCollection<object>> ViewWorkItemCommand { get; private set; }

        private void ViewWorkItem(ObservableCollection<object> collection)
        {
            foreach (var workItemModel in collection.Cast<WorkItemModel>())
            {
                OpenWorkItemHelperWrapper.OpenWorkItem(workItemModel.WorkItem, collection.Count == 1);
            }
        }

        private bool CanViewWorkItem(ObservableCollection<object> collection)
        {
            return collection != null && collection.Any();
        }

        #endregion

        #region CompareWithWorkspace Command

        public RelayCommand<ObservableCollection<object>> CompareWithWorkspaceCommand { get; private set; }

        private void CompareWithWorkspace(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.CompareChangesetChangesWithWorkspaceVersions(Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanCompareWithWorkspace(ObservableCollection<object> collection)
        {
            return collection != null && collection.Count == 1;
        }

        #endregion

        #region CompareWithLatest Command

        public RelayCommand<ObservableCollection<object>> CompareWithLatestCommand { get; private set; }

        private void CompareWithLatest(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.CompareChangesetChangesWithLatestVersions(Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanCompareWithLatest(ObservableCollection<object> collection)
        {
            return collection != null && collection.Count == 1;
        }

        #endregion

        #region UndoPendingChange Command

        public RelayCommand<ObservableCollection<object>> UndoPendingChangeCommand { get; private set; }

        private void UndoPendingChange(ObservableCollection<object> collection)
        {
            teamPilgrimVsService.UndoChanges(Workspace, collection.Cast<PendingChangeModel>().Select(model => model.Change).ToArray());
        }

        private bool CanUndoPendingChange(ObservableCollection<object> collection)
        {
            return collection != null && collection.Any();
        }

        #endregion

        #region PendingChangeProperties Command

        public RelayCommand<ObservableCollection<object>> PendingChangePropertiesCommand { get; private set; }

        private void PendingChangeProperties(ObservableCollection<object> collection)
        {
            //TODO: Implement PendingChangeProperties
        }

        private bool CanPendingChangeProperties(ObservableCollection<object> collection)
        {
            return collection != null && collection.Count == 1;
        }

        #endregion

        #region CheckIn Command

        public RelayCommand CheckInCommand { get; private set; }

        private void CheckIn()
        {
            this.Logger().Trace("CheckIn");

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
                OnShowPendingChangesItem(ShowPendingChangesTabItemEnum.CheckinNotes);

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
                this.Logger().Debug("CheckIn EvaluateCheckin: Valid:{0}", checkinEvaluationResult.IsValid());

                PolicyOverrideInfo policyOverrideInfo = null;

                if (!checkinEvaluationResult.IsValid())
                {
                    if (checkinEvaluationResult.Conflicts.Any())
                    {
                        MessageBox.Show(
                            "Check In\r\n\r\nNo files checked in due to conflicting changes. Please use Conflicts Manager to resolve conflicts and try again.",
                            "Team Pilgrim", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                        var conflictedServerItems = checkinEvaluationResult.Conflicts.Select(conflict => conflict.ServerItem).ToArray();
                        teamPilgrimVsService.ResolveConflicts(Workspace, conflictedServerItems, false, false);

                        return;
                    }

                    if (checkinEvaluationResult.PolicyFailures.Any())
                    {
                        OnShowPendingChangesItem(ShowPendingChangesTabItemEnum.PolicyWarnings);

                        var policyFailureModel = new PolicyFailureModel();
                        var policyFailureDialog = new PolicyFailureDialog()
                            {
                                DataContext = policyFailureModel
                            };

                        var dialogResult = policyFailureDialog.ShowDialog();
                        if (!dialogResult.HasValue || !dialogResult.Value || !policyFailureModel.Override)
                        {
                            CheckinEvaluationResult = checkinEvaluationResult;
                            return;
                        }

                        policyOverrideInfo = new PolicyOverrideInfo(policyFailureModel.Reason, checkinEvaluationResult.PolicyFailures);
                    }
                }

                if (teamPilgrimServiceModelProvider.TryCheckin(Workspace, pendingChanges, Comment, checkinNote, workItemChanges, policyOverrideInfo))
                {
                    Comment = string.Empty;
                    RefreshPendingChanges();

                    foreach (var workItem in WorkItems.Where(model => model.IsSelected))
                    {
                        workItem.IsSelected = false;
                    }

                    RefreshPendingChangesCommand.Execute(null);
                    RefreshSelectedDefinitionWorkItemsCommand.Execute(null);
                }
            }
        }

        private bool CanCheckIn()
        {
            return PendingChanges.Any(model => model.IncludeChange);
        }

        #endregion

        #region EvaluateCheckIn Command

        public RelayCommand EvaluateCheckInCommand { get; private set; }

        private void EvaluateCheckIn()
        {
            this.Logger().Trace("EvaluateCheckIn");

            var pendingChanges = PendingChanges
                                    .Where(model => model.IncludeChange)
                                    .Select(model => model.Change)
                                    .ToArray();

            if (!pendingChanges.Any())
            {
                CheckinEvaluationResult = null;
                CheckinNotes.Clear();
                return;
            }

            var currentCheckinNoteDefinitions = checkinNotesCacheWrapper.GetCheckinNotes(pendingChanges);

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
                this.Logger().Debug("EvaluateCheckIn: Valid:{0}", checkinEvaluationResult.IsValid());
            }
        }

        private bool CanEvaluateCheckIn()
        {
            var canEvaluateCheckIn = !_backgroundFunctionPreventDataUpdate;

            return canEvaluateCheckIn;
        }

        #endregion

        #region RefreshPendingChanges Command

        public RelayCommand RefreshPendingChangesCommand { get; private set; }

        private void RefreshPendingChanges()
        {
            this.Logger().Trace("RefreshPendingChanges");

            PendingChange[] currentPendingChanges;

            var filterItems = SolutionIsOpen && FilterSolution;
            string[] solutionFilePaths = null;

            if (filterItems)
            {
                try
                {
                    solutionFilePaths = teamPilgrimVsService.GetSolutionFilePaths();
                }
                catch (Exception)
                {
                    filterItems = false;
                }
            }

            if (filterItems
                ? teamPilgrimServiceModelProvider.TryGetPendingChanges(out currentPendingChanges, Workspace, solutionFilePaths)
                : teamPilgrimServiceModelProvider.TryGetPendingChanges(out currentPendingChanges, Workspace))
            {
                var intersections = PendingChanges
                    .Join(currentPendingChanges, model => model.Change.ItemId, change => change.ItemId, (model, change) => new { model, change })
                    .ToArray();

                var intersectedModels =
                    intersections
                    .Select(arg => arg.model)
                    .ToArray();

                var modelsToRemove = PendingChanges.Where(model => !intersectedModels.Contains(model)).ToArray();

                var modelsToAdd = currentPendingChanges
                    .Where(pendingChange => !intersectedModels.Select(model => model.Change.ItemId).Contains(pendingChange.ItemId))
                    .Select(change => new PendingChangeModel(change)).ToArray();

                _backgroundFunctionPreventDataUpdate = true;

                foreach (var intersection in intersections)
                {
                    intersection.model.Change = intersection.change;
                }

                foreach (var modelToAdd in modelsToAdd)
                {
                    PendingChanges.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    PendingChanges.Remove(modelToRemove);
                }

                _backgroundFunctionPreventDataUpdate = false;

                PopulateSelectedPendingChangesSummary();
                EvaluateCheckInCommand.Execute(null);
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
            this.Logger().Trace("RefreshSelectedDefinitionWorkItems");

            if (SelectedWorkItemQueryDefinition == null)
                return;

            WorkItemCollection workItemCollection;
            if (teamPilgrimServiceModelProvider.TryGetQueryDefinitionWorkItemCollection(out workItemCollection,
                                                                                        _projectCollectionServiceModel.TfsTeamProjectCollection,
                                                                                        SelectedWorkItemQueryDefinition.QueryDefinition,
                                                                                        SelectedWorkItemQueryDefinition.Project.Name))
            {
                var currentWorkItems = workItemCollection.Cast<WorkItem>().ToArray();

                var intersections = WorkItems
                    .Join(currentWorkItems, model => model.WorkItem.Id, workItem => workItem.Id, (model, workitem) => new { model, workitem })
                    .ToArray();

                foreach (var intersection in intersections)
                {
                    intersection.model.WorkItem = intersection.workitem;
                }

                var intersectedModels =
                    intersections
                    .Select(arg => arg.model)
                        .ToArray();

                var modelsToRemove = WorkItems.Where(model => !intersectedModels.Contains(model)).ToArray();

                var selectedWorkItemCheckinActionEnum = TeamPilgrimPackage.TeamPilgrimSettings.SelectedWorkItemCheckinAction;
                var modelsToAdd = currentWorkItems
                    .Where(workItem => !intersectedModels.Select(workItemModel => workItemModel.WorkItem.Id).Contains(workItem.Id))
                    .Select(workItem => new WorkItemModel(workItem) { WorkItemCheckinAction = selectedWorkItemCheckinActionEnum }).ToArray();

                _backgroundFunctionPreventDataUpdate = false;

                foreach (var modelToAdd in modelsToAdd)
                {
                    WorkItems.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    WorkItems.Remove(modelToRemove);
                }

                _backgroundFunctionPreventDataUpdate = false;
                EvaluateCheckInCommand.Execute(null);
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
            //TODO: This should be an event, and the dialog should be displayed by a control object

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

        #region ShelveCommand Command

        public RelayCommand ShelveCommand { get; private set; }

        protected virtual void OnShowShelveDialog(ShelvesetServiceModel shelvesetServiceModel)
        {
            var handler = ShowShelveDialog;
            if (handler != null) handler(shelvesetServiceModel);
        }

        private void Shelve()
        {
            OnShowShelveDialog(new ShelvesetServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, _projectCollectionServiceModel, this));
        }

        private bool CanShelve()
        {
            return PendingChanges.Any();
        }

        #endregion

        #region Unshelve Command

        public RelayCommand UnshelveCommand { get; private set; }

        protected virtual void OnShowUnshelveDialog()
        {
            var handler = ShowUnshelveDialog;
            if (handler != null) handler();
        }

        private void Unshelve()
        {
            OnShowUnshelveDialog();
        }

        private bool CanUnshelve()
        {
            return true;
        }

        #endregion
    }
}