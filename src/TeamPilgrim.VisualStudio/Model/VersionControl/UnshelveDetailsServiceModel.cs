using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class UnshelveDetailsServiceModel : BaseServiceModel
    {
        public WorkspaceServiceModel WorkspaceServiceModel { get; private set; }
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

        public UnshelveDetailsServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, WorkspaceServiceModel workspaceServiceModel, Shelveset shelveset)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            WorkspaceServiceModel = workspaceServiceModel;
            Shelveset = shelveset;

            PendingChanges = new ObservableCollection<PendingChangeModel>();

            CancelCommand = new RelayCommand(Cancel, CanCancel);

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
            Messenger.Default.Send(new DismissMessage { Success = false }, this);
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion
    }
}

