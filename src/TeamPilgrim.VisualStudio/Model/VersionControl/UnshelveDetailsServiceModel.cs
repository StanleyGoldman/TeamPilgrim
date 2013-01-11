using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages.Dismiss;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class UnshelveDetailsServiceModel : BaseServiceModel
    {
        public ProjectCollectionServiceModel ProjectCollectionServiceModel { get; set; }
        public WorkspaceServiceModel WorkspaceServiceModel { get; private set; }
        public UnshelveServiceModel UnshelveServiceModel { get; set; }
        public Shelveset Shelveset { get; private set; }
        public PendingSet PendingSet { get; private set; }

        private bool _restoreWorkItemsAndCheckinNotes = true;
        public bool RestoreWorkItemsAndCheckinNotes
        {
            get { return _restoreWorkItemsAndCheckinNotes; }
            set
            {
                if (_restoreWorkItemsAndCheckinNotes == value)
                    return;

                _restoreWorkItemsAndCheckinNotes = value;

                SendPropertyChanged("RestoreWorkItemsAndCheckinNotes");
            }
        }

        private bool _preserveShelveset = true;
        public bool PreserveShelveset
        {
            get { return _preserveShelveset; }
            set
            {
                if (_preserveShelveset == value)
                    return;

                _preserveShelveset = value;

                SendPropertyChanged("PreserveShelveset");
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

        public UnshelveDetailsServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionServiceModel projectCollectionServiceModel, WorkspaceServiceModel workspaceServiceModel, UnshelveServiceModel unshelveServiceModel, Shelveset shelveset)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectCollectionServiceModel = projectCollectionServiceModel;
            WorkspaceServiceModel = workspaceServiceModel;
            UnshelveServiceModel = unshelveServiceModel;
            Shelveset = shelveset;

            PendingChanges = new ObservableCollection<PendingChangeModel>();

            CancelCommand = new RelayCommand(Cancel, CanCancel);
            UnshelveCommand = new RelayCommand(Unshelve, CanUnshelve);

            SelectPendingChangesCommand = new RelayCommand<SelectPendingChangesCommandArgument>(SelectPendingChanges, CanSelectPendingChanges);
            SelectWorkItemsCommand = new RelayCommand<SelectWorkItemsCommandArgument>(SelectWorkItems, CanSelectWorkItems);

            PendingSet[] pendingSets;
            if (teamPilgrimServiceModelProvider.TryWorkspaceQueryShelvedChanges(WorkspaceServiceModel.Workspace, out pendingSets, shelveset.Name, shelveset.OwnerName, null))
            {
                PendingSet = pendingSets.First();

                foreach (var pendingChange in PendingSet.PendingChanges)
                {
                    PendingChanges.Add(new PendingChangeModel(pendingChange) { IncludeChange = true });
                }
            }

            PopulateSelectedPendingChangesSummary();
        }

        protected virtual void OnDismiss(bool success, bool dismissUnshelveModel)
        {
            Messenger.Default.Send(new DismissUnshelveDetailsMessage { Success = success, DismissUnshelveModel = dismissUnshelveModel }, this);
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

        #region PendingChanges Collection

        public ObservableCollection<PendingChangeModel> PendingChanges { get; private set; }

        #endregion

        #region Cancel Command

        public RelayCommand CancelCommand { get; private set; }

        public void Cancel()
        {
            OnDismiss(false, false);
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion

        #region Unshelve Command

        public RelayCommand UnshelveCommand { get; private set; }

        public void Unshelve()
        {
            Shelveset shelveset;
            bool result;
            if (PendingChanges.All(model => model.IncludeChange))
            {
                result = teamPilgrimServiceModelProvider.TryWorkspaceUnshelve(WorkspaceServiceModel.Workspace, out shelveset, Shelveset.Name, Shelveset.OwnerName);
            }
            else
            {
                var itemSpecs = PendingChanges
                    .Where(model => model.IncludeChange)
                    .Select(model => new ItemSpec(model.Change))
                    .ToArray();

                result = teamPilgrimServiceModelProvider.TryWorkspaceUnshelve(WorkspaceServiceModel.Workspace, out shelveset,
                                                                 Shelveset.Name, Shelveset.OwnerName, itemSpecs);
            }

            if (result)
            {
                if (RestoreWorkItemsAndCheckinNotes)
                {
                    foreach (var workItemCheckinInfo in Shelveset.WorkItemInfo)
                    {
                        WorkspaceServiceModel.SelectWorkItemById(workItemCheckinInfo.WorkItem.Id);
                    }

                    foreach (var checkinNoteFieldValue in this.Shelveset.CheckinNote.Values)
                    {
                        WorkspaceServiceModel.RestoreCheckinNoteFieldValue(checkinNoteFieldValue);
                    }
                }

                if (!PreserveShelveset)
                {
                    teamPilgrimServiceModelProvider.TryDeleteShelveset(ProjectCollectionServiceModel.TfsTeamProjectCollection, shelveset.Name, shelveset.OwnerName);
                }
            }

            OnDismiss(result, true);
        }

        public bool CanUnshelve()
        {
            return PendingChanges.Any(model => model.IncludeChange);
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
        }

        private bool CanSelectWorkItems(SelectWorkItemsCommandArgument commandArgument)
        {
            return commandArgument.Collection != null && commandArgument.Collection.Any();
        }

        #endregion
    }
}

