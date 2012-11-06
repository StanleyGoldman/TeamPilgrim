using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkspaceModel : BaseModel
    {
        private readonly TeamPilgrimModel _teamPilgrimModel;

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

        public WorkspaceModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimModel teamPilgrimModel, Workspace workspace)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            _teamPilgrimModel = teamPilgrimModel;
            Workspace = workspace;
            
            CheckInCommand = new RelayCommand(CheckIn, CanCheckIn);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            ShowSelectWorkItemQueryCommand = new RelayCommand(ShowSelectWorkItemQuery, CanShowSelectWorkItemQuery);

            PendingChanges = new ObservableCollection<PendingChangeModel>();

            PendingChange[] pendingChanges;
            if (pilgrimServiceModelProvider.TryGetPendingChanges(out pendingChanges, Workspace))
            {
                foreach (var pendingChange in pendingChanges)
                {
                    var pendingChangeModel = new PendingChangeModel(pilgrimServiceModelProvider, teamPilgrimVsService, pendingChange);
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

            if (pilgrimServiceModelProvider.TryWorkspaceCheckin(Workspace, pendingChanges, Comment))
            {
                Comment = string.Empty;
                Refresh();
            }
        }

        private bool CanCheckIn()
        {
            return true;
        }

        #endregion

        #region Refresh Command

        public RelayCommand RefreshCommand { get; private set; }

        private void Refresh()
        {
            PendingChange[] currentPendingChanges;
            if (pilgrimServiceModelProvider.TryGetPendingChanges(out currentPendingChanges, Workspace))
            {
                var modelIntersection =
                    PendingChanges
                    .Join(currentPendingChanges, model => model.Change.PendingChangeId, change => change.PendingChangeId, (model, change) => model)
                    .ToArray();

                var modelsToRemove = PendingChanges.Where(model => !modelIntersection.Contains(model)).ToArray();

                var modelsToAdd = currentPendingChanges
                    .Where(pendingChange => !modelIntersection.Select(model => model.Change.PendingChangeId).Contains(pendingChange.PendingChangeId))
                    .Select(change => new PendingChangeModel(pilgrimServiceModelProvider, teamPilgrimVsService, change)).ToArray();

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

        private bool CanRefresh()
        {
            return true;
        }

        #endregion

        #region ShowSelectWorkItemQuery Command

        public RelayCommand ShowSelectWorkItemQueryCommand { get; private set; }

        private void ShowSelectWorkItemQuery()
        {
            var selectWorkItemQueryDialog = new SelectWorkItemQueryDialog
                {
                    DataContext = new SelectWorkItemQueryModel(pilgrimServiceModelProvider, teamPilgrimVsService, _teamPilgrimModel.ActiveProjectCollectionModel)
                };
            
            var result = selectWorkItemQueryDialog.ShowDialog();
        }

        private bool CanShowSelectWorkItemQuery()
        {
            return true;
        }

        #endregion
    }
}