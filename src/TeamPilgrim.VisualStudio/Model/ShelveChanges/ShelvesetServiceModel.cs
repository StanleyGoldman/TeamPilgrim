using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NLog;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ShelveChanges
{
    public class ShelvesetServiceModel : BaseServiceModel
    {
        private static readonly Logger Logger = TeamPilgrimLogManager.Instance.GetCurrentClassLogger();

        public delegate void ShowPendingChangesItemDelegate(ShowPendingChangesTabItemEnum showPendingChangesTabItemEnum);
        public event ShowPendingChangesItemDelegate ShowPendingChangesItem;

        public delegate void DismissDelegate(bool success);
        public event DismissDelegate Dismiss;

        public ObservableCollection<CheckinNoteModel> CheckinNotes { get; private set; }

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

                var previousValue = _comment;

                _comment = value;

                SendPropertyChanged("Comment");

                if (string.IsNullOrWhiteSpace(previousValue) ^ string.IsNullOrWhiteSpace(_comment))
                {
                    Logger.Debug("Comment IsNullOrWhiteSpace Status Changed");
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

        private bool _evaluatePoliciesAndCheckinNotes;
        public bool EvaluatePoliciesAndCheckinNotes
        {
            get
            {
                return _evaluatePoliciesAndCheckinNotes;
            }
            set
            {
                if (_evaluatePoliciesAndCheckinNotes == value) return;

                _evaluatePoliciesAndCheckinNotes = value;

                SendPropertyChanged("EvaluatePoliciesAndCheckinNotes");
            }
        }

        private bool _preservePendingChangesLocally = TeamPilgrimPackage.TeamPilgrimSettings.PreserveShelvedChangesLocally;
        public bool PreservePendingChangesLocally
        {
            get
            {
                return _preservePendingChangesLocally;
            }
            set
            {
                if (_preservePendingChangesLocally == value) return;

                _preservePendingChangesLocally = value;

                SendPropertyChanged("PreservePendingChangesLocally");
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

                TeamPilgrimPackage.TeamPilgrimSettings.AddPreviouslySelectedWorkItemQuery(_projectCollectionServiceModel.TfsTeamProjectCollection.Uri.ToString(), value.QueryDefinition.Path);
                PopulatePreviouslySelectedWorkItemQueryModels();
            }
        }

        private bool _backgroundFunctionPreventEvaluateCheckin;

        public ShelvesetServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider,
                                     ITeamPilgrimVsService teamPilgrimVsService,
                                     ProjectCollectionServiceModel projectCollectionServiceModel, WorkspaceServiceModel workspaceServiceModel)

            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _projectCollectionServiceModel = projectCollectionServiceModel;
            _workspaceServiceModel = workspaceServiceModel;

            var versionControlServer = _projectCollectionServiceModel.TfsTeamProjectCollection.GetService<VersionControlServer>();
            versionControlServer.PendingChangesChanged += VersionControlServerOnPendingChangesChanged;

            _filterSolution = workspaceServiceModel.FilterSolution;
            _selectedWorkWorkItemQueryDefinition = workspaceServiceModel.SelectedWorkItemQueryDefinition;
            _comment = workspaceServiceModel.Comment;

            EvaluateCheckInCommand = new RelayCommand(EvaluateCheckIn, CanEvaluateCheckIn);
            ShowSelectWorkItemQueryCommand = new RelayCommand(ShowSelectWorkItemQuery, CanShowSelectWorkItemQuery);
            RefreshPendingChangesCommand = new RelayCommand(RefreshPendingChanges, CanRefreshPendingChanges);
            RefreshSelectedDefinitionWorkItemsCommand = new RelayCommand(RefreshSelectedDefinitionWorkItems, CanRefreshSelectedDefinitionWorkItems);
            ShelveCommand = new RelayCommand(Shelve, CanShelve);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            SelectPendingChangesCommand = new RelayCommand<SelectPendingChangesCommandArgument>(SelectPendingChanges, CanSelectPendingChanges);
            SelectWorkItemsCommand = new RelayCommand<SelectWorkItemsCommandArgument>(SelectWorkItems, CanSelectWorkItems);

            CompareWithLatestCommand = new RelayCommand<ObservableCollection<object>>(CompareWithLatest, CanCompareWithLatest);
            CompareWithWorkspaceCommand = new RelayCommand<ObservableCollection<object>>(CompareWithWorkspace, CanCompareWithWorkspace);
            UndoPendingChangeCommand = new RelayCommand<ObservableCollection<object>>(UndoPendingChange, CanUndoPendingChange);
            PendingChangePropertiesCommand = new RelayCommand<ObservableCollection<object>>(PendingChangeProperties, CanPendingChangeProperties);

            CheckinNotes = new ObservableCollection<CheckinNoteModel>();

            PendingChanges = new TrulyObservableCollection<PendingChangeModel>(workspaceServiceModel.PendingChanges.Select(model => new PendingChangeModel(model.Change)
                {
                    IncludeChange = model.IncludeChange
                }).ToList());
            PendingChanges.CollectionChanged += PendingChangesOnCollectionChanged;

            WorkItems = new TrulyObservableCollection<WorkItemModel>(workspaceServiceModel.WorkItems.Select(model => new WorkItemModel(model.WorkItem)
                {
                    IsSelected = model.IsSelected,
                    WorkItemCheckinAction = model.WorkItemCheckinAction
                }).ToList());
            WorkItems.CollectionChanged += WorkItemsOnCollectionChanged;

            PopulatePreviouslySelectedWorkItemQueryModels();

            EvaluateCheckInCommand.Execute(null);
        }
      
        private void VersionControlServerOnPendingChangesChanged(object sender, WorkspaceEventArgs workspaceEventArgs)
        {
            Logger.Debug("VersionControlServerOnPendingChangesChanged");

            RefreshPendingChanges();
        }

        private void PopulatePreviouslySelectedWorkItemQueryModels()
        {
            var previouslySelectedWorkItemsQuery = TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemsQueries[_projectCollectionServiceModel.TfsTeamProjectCollection.Uri.ToString()];
            PreviouslySelectedWorkItemQueries = previouslySelectedWorkItemsQuery.Select(workItemQueryPath => new PreviouslySelectedWorkItemQuery(workItemQueryPath)).ToArray();
        }

        protected virtual void OnShowPendingChangesItem(ShowPendingChangesTabItemEnum showpendingchangestabitemenum)
        {
            var handler = ShowPendingChangesItem;
            if (handler != null) handler(showpendingchangestabitemenum);
        }

        protected virtual void OnDismiss(bool success)
        {
            var handler = Dismiss;
            if (handler != null) handler(success);
        }

        #region PendingChanges Collection

        public TrulyObservableCollection<PendingChangeModel> PendingChanges { get; private set; }

        private void PendingChangesOnCollectionChanged(object sender,
                                                       NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Logger.Trace("PendingChangesOnCollectionChanged");

            EvaluateCheckInCommand.Execute(null);
        }

        #endregion

        #region WorkItems Collection

        public TrulyObservableCollection<WorkItemModel> WorkItems { get; private set; }

        private void WorkItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Logger.Trace("WorkItemsOnCollectionChanged");

            EvaluateCheckInCommand.Execute(null);
        }

        #endregion

        #region SelectPendingChanges Command

        public RelayCommand<SelectPendingChangesCommandArgument> SelectPendingChangesCommand { get; private set; }

        private void SelectPendingChanges(SelectPendingChangesCommandArgument selectPendingChangesCommandArgument)
        {
            Logger.Debug("Select Pending Changes: {0}, Count: {1}", selectPendingChangesCommandArgument.Value, selectPendingChangesCommandArgument.Collection.Count());

            _backgroundFunctionPreventEvaluateCheckin = true;

            foreach (var pendingChangeModel in selectPendingChangesCommandArgument.Collection)
            {
                pendingChangeModel.IncludeChange = selectPendingChangesCommandArgument.Value;
            }

            _backgroundFunctionPreventEvaluateCheckin = false;

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
            Logger.Debug("Select Work Items: {0}, Count: {1}", selectWorkItemsCommandArgument.Value, selectWorkItemsCommandArgument.Collection.Count());

            _backgroundFunctionPreventEvaluateCheckin = true;

            foreach (var workItemModel in selectWorkItemsCommandArgument.Collection)
            {
                workItemModel.IsSelected = selectWorkItemsCommandArgument.Value;
            }

            _backgroundFunctionPreventEvaluateCheckin = false;
            EvaluateCheckInCommand.Execute(null);
        }

        private bool CanSelectWorkItems(SelectWorkItemsCommandArgument commandArgument)
        {
            return commandArgument.Collection != null && commandArgument.Collection.Any();
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
            return collection != null && collection.Count == 1;
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
            return collection != null && collection.Count == 1;
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
            return collection != null && collection.Count == 1;
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

        #region RefreshPendingChanges Command

        public RelayCommand RefreshPendingChangesCommand { get; private set; }

        private void RefreshPendingChanges()
        {
            Logger.Trace("RefreshPendingChanges");

            PendingChange[] currentPendingChanges;

            if (_projectCollectionServiceModel.TeamPilgrimServiceModel.SolutionIsOpen && FilterSolution
                ? teamPilgrimServiceModelProvider.TryGetPendingChanges(out currentPendingChanges, _workspaceServiceModel.Workspace, teamPilgrimVsService.GetSolutionFilePaths())
                : teamPilgrimServiceModelProvider.TryGetPendingChanges(out currentPendingChanges, _workspaceServiceModel.Workspace))
            {
                var modelIntersection =
                    PendingChanges
                    .Join(currentPendingChanges, model => model.Change.ItemId, change => change.ItemId, (model, change) => model)
                    .ToArray();

                var modelsToRemove = PendingChanges.Where(model => !modelIntersection.Contains(model)).ToArray();

                var modelsToAdd = currentPendingChanges
                    .Where(pendingChange => !modelIntersection.Select(model => model.Change.ItemId).Contains(pendingChange.ItemId))
                    .Select(change => new PendingChangeModel(change)).ToArray();

                _backgroundFunctionPreventEvaluateCheckin = true;

                foreach (var modelToAdd in modelsToAdd)
                {
                    PendingChanges.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    PendingChanges.Remove(modelToRemove);
                }

                _backgroundFunctionPreventEvaluateCheckin = false;

                EvaluateCheckInCommand.Execute(null);
            }
        }

        private bool CanRefreshPendingChanges()
        {
            return true;
        }

        #endregion

        #region Shelve Command

        public RelayCommand ShelveCommand { get; private set; }

        private void Shelve()
        {
            var pendingChanges = PendingChanges
                .Where(model => model.IncludeChange)
                .Select(model => model.Change)
                .ToArray();

            if (EvaluatePoliciesAndCheckinNotes)
            {
                var missingCheckinNotes = CheckinNotes
                    .Where(model => model.CheckinNoteFieldDefinition.Required && string.IsNullOrWhiteSpace(model.Value))
                    .Select(model => model.CheckinNoteFieldDefinition.Name).ToArray();

                if (missingCheckinNotes.Any())
                {
                    OnShowPendingChangesItem(ShowPendingChangesTabItemEnum.CheckinNotes);

                    MessageBox.Show("Check-in Validation\r\n\r\nEnter a value for " + string.Join(", ", missingCheckinNotes), "Team Pilgrim", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            VersionControlServer versionControlServer;
            if (teamPilgrimServiceModelProvider.TryGetVersionControlServer(out versionControlServer, _projectCollectionServiceModel.TfsTeamProjectCollection))
            {
                Debug.Assert(versionControlServer != null, "versionControlServer != null");

                var workItemInfo = WorkItems
                    .Where(model => model.IsSelected)
                    .Select(model => new WorkItemCheckinInfo(model.WorkItem, model.WorkItemCheckinAction.ToWorkItemCheckinAction()))
                    .ToArray();

                var checkinNoteFieldValues =
                    CheckinNotes
                    .Where(model => !string.IsNullOrWhiteSpace(model.Value))
                    .Select(model => new CheckinNoteFieldValue(model.CheckinNoteFieldDefinition.Name, model.Value))
                    .ToArray();

                var checkinNote = new CheckinNote(checkinNoteFieldValues);

                string policyOverrideComment = null;
                if (EvaluatePoliciesAndCheckinNotes)
                {
                    CheckinEvaluationResult checkinEvaluationResult;
                    if (teamPilgrimServiceModelProvider.TryEvaluateCheckin(out checkinEvaluationResult, _workspaceServiceModel.Workspace, pendingChanges, Comment, checkinNote, workItemInfo))
                    {
                        if (!checkinEvaluationResult.IsValid())
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

                            policyOverrideComment = policyFailureModel.Reason;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                var shelveset = new Shelveset(versionControlServer, ShelvesetName, _projectCollectionServiceModel.TfsTeamProjectCollection.AuthorizedIdentity.UniqueName)
                    {
                        Comment = Comment,
                        ChangesExcluded = PendingChanges.Count() != pendingChanges.Count(),
                        WorkItemInfo = workItemInfo,
                        CheckinNote = checkinNote,
                        PolicyOverrideComment = policyOverrideComment
                    };

                PendingSet[] pendingSets;
                if (teamPilgrimServiceModelProvider.TryWorkspaceQueryShelvedChanges(_workspaceServiceModel.Workspace, out pendingSets, ShelvesetName,
                                                                                    _projectCollectionServiceModel.TfsTeamProjectCollection.AuthorizedIdentity.UniqueName, null))
                {
                    bool overwrite = false;
                    if (pendingSets != null && pendingSets.Any())
                    {
                        if (MessageBox.Show(string.Format("Replace shelveset\r\n\r\nThe shelveset {0} already exists. Replace?", ShelvesetName),
                                "Team Pilgrim", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            overwrite = true;
                        }
                        else
                        {
                            return;
                        }
                    }

                    var shelvingOptions = ShelvingOptions.None;

                    if (!PreservePendingChangesLocally)
                        shelvingOptions |= ShelvingOptions.Move;

                    if (overwrite)
                        shelvingOptions |= ShelvingOptions.Replace;

                    if (teamPilgrimServiceModelProvider.TryShelve(_workspaceServiceModel.Workspace, shelveset, pendingChanges, shelvingOptions))
                    {
                    }
                }
            }

            OnDismiss(true);
        }

        private bool CanShelve()
        {
            return PendingChanges.Any(model => model.IncludeChange);
        }

        #endregion

        #region Cancel Command

        public RelayCommand CancelCommand { get; private set; }

        public void Cancel()
        {
            OnDismiss(false);
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion

        #region RefreshSelectedDefinitionWorkItemsCommand Command

        public RelayCommand RefreshSelectedDefinitionWorkItemsCommand { get; private set; }

        private void RefreshSelectedDefinitionWorkItems()
        {
            Logger.Trace("RefreshSelectedDefinitionWorkItems");

            if (SelectedWorkItemQueryDefinition == null)
                return;

            WorkItemCollection workItemCollection;
            if (teamPilgrimServiceModelProvider.TryGetQueryDefinitionWorkItemCollection(out workItemCollection,
                                                                                        _projectCollectionServiceModel.TfsTeamProjectCollection,
                                                                                        SelectedWorkItemQueryDefinition.QueryDefinition,
                                                                                        SelectedWorkItemQueryDefinition.Project.Name))
            {
                var currentWorkItems = workItemCollection.Cast<WorkItem>().ToArray();

                var modelIntersection = WorkItems == null
                                            ? new WorkItemModel[0]
                                            : WorkItems
                                                    .Join(currentWorkItems, model => model.WorkItem.Id,
                                                        workItem => workItem.Id,
                                                        (model, change) => model)
                                                    .ToArray();

                var modelsToRemove = WorkItems.Where(model => !modelIntersection.Contains(model)).ToArray();

                var selectedWorkItemCheckinActionEnum = TeamPilgrimPackage.TeamPilgrimSettings.SelectedWorkItemCheckinAction;
                var modelsToAdd = currentWorkItems
                    .Where(workItem => !modelIntersection.Select(workItemModel => workItemModel.WorkItem.Id).Contains(workItem.Id))
                    .Select(workItem => new WorkItemModel(workItem) { WorkItemCheckinAction = selectedWorkItemCheckinActionEnum }).ToArray();

                _backgroundFunctionPreventEvaluateCheckin = false;

                foreach (var modelToAdd in modelsToAdd)
                {
                    WorkItems.Add(modelToAdd);
                }

                foreach (var modelToRemove in modelsToRemove)
                {
                    WorkItems.Remove(modelToRemove);
                }

                _backgroundFunctionPreventEvaluateCheckin = false;
                EvaluateCheckInCommand.Execute(null);
            }
        }

        private bool CanRefreshSelectedDefinitionWorkItems()
        {
            return true;
        }

        #endregion

        #region EvaluateCheckIn Command

        public RelayCommand EvaluateCheckInCommand { get; private set; }

        private void EvaluateCheckIn()
        {
            Logger.Trace("EvaluateCheckIn");

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

            var currentCheckinNoteDefinitions = _workspaceServiceModel.checkinNotesCacheWrapper.GetCheckinNotes(pendingChanges);

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

            if (teamPilgrimServiceModelProvider.TryEvaluateCheckin(out checkinEvaluationResult, _workspaceServiceModel.Workspace, pendingChanges, Comment, checkinNote, workItemChanges))
            {
                CheckinEvaluationResult = checkinEvaluationResult;
                Logger.Debug("EvaluateCheckIn: Valid:{0}", checkinEvaluationResult.IsValid());
            }
        }

        private bool CanEvaluateCheckIn()
        {
            var canEvaluateCheckIn = !_backgroundFunctionPreventEvaluateCheckin;

            Logger.Trace("CanEvaluateCheckIn: Result: {0}", canEvaluateCheckIn);

            return canEvaluateCheckIn;
        }

        #endregion
    }
}
