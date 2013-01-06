using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class UnshelveServiceModel : BaseServiceModel
    {
        public ProjectCollectionServiceModel ProjectCollectionServiceModel { get; private set; }
        public WorkspaceServiceModel WorkspaceServiceModel { get; private set; }

        private string _shelvesetOwner;
        public string ShelvesetOwner
        {
            get
            {
                return _shelvesetOwner;
            }
            private set
            {
                if (_shelvesetOwner == value) return;

                _shelvesetOwner = value;

                SendPropertyChanged("ShelvesetOwner");
            }
        }

        private ShelvesetModel[] _shelvesets;
        public ShelvesetModel[] Shelvesets
        {
            get { return _shelvesets; }
            set
            {
                if (_shelvesets == value) return;

                _shelvesets = value;

                SendPropertyChanged("Shelvesets");
            }
        }

        public UnshelveServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider,
                                           ITeamPilgrimVsService teamPilgrimVsService,
                                           ProjectCollectionServiceModel projectCollectionServiceModel, WorkspaceServiceModel workspaceServiceModel)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectCollectionServiceModel = projectCollectionServiceModel;
            WorkspaceServiceModel = workspaceServiceModel;

            ShelvesetOwner = projectCollectionServiceModel.TfsTeamProjectCollection.AuthorizedIdentity.UniqueName;

            FindShelvesetsCommand = new RelayCommand<string>(FindShelvesets, CanFindShelvesets);
            ViewPendingSetCommand = new RelayCommand<ShelvesetModel>(ViewPendingSet, CanViewPendingSet);
            UnshelveCommand = new RelayCommand<ObservableCollection<object>>(Unshelve, CanUnshelve);
            DeleteCommand = new RelayCommand<ObservableCollection<object>>(Delete, CanDelete);
            DetailsCommand = new RelayCommand<ObservableCollection<object>>(Details, CanDetails);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            FindShelvesetsCommand.Execute(null);
        }

        protected virtual void OnDismiss(bool success)
        {
            Messenger.Default.Send(new DismissMessage { Success = success }, this);
        }

        #region FindShelvesets Command

        public RelayCommand<string> FindShelvesetsCommand { get; private set; }

        private void FindShelvesets(string username)
        {
            this.Logger().Trace("FindShelvesets");

            Shelveset[] shelvesets;
            if (teamPilgrimServiceModelProvider.TryQueryShelvesets(ProjectCollectionServiceModel.TfsTeamProjectCollection, out shelvesets, shelvesetOwner: ShelvesetOwner))
            {
                Shelvesets = shelvesets.Select(shelveset => new ShelvesetModel(shelveset)).ToArray();
            }
        }

        private bool CanFindShelvesets(string username)
        {
            return true;
        }

        #endregion

        #region ViewPendingSet Command

        public RelayCommand<ShelvesetModel> ViewPendingSetCommand { get; private set; }

        private void ViewPendingSet(ShelvesetModel shelvesetModel)
        {
            this.Logger().Trace("ViewPendingSet");
        }

        private bool CanViewPendingSet(ShelvesetModel shelvesetModel)
        {
            return true;
        }

        #endregion

        #region Unshelve Command

        public RelayCommand<ObservableCollection<object>> UnshelveCommand { get; private set; }

        private void Unshelve(ObservableCollection<object> shelvesetModels)
        {
            var success = false;

            Shelveset shelveset;
            var shelvesetModel = (ShelvesetModel) shelvesetModels.First();
            if (teamPilgrimServiceModelProvider.TryWorkspaceUnshelve(WorkspaceServiceModel.Workspace, out shelveset,
                                                                     shelvesetModel.Shelveset.Name, shelvesetModel.Shelveset.OwnerName))
            {
                success = true;
            }

            OnDismiss(success);
        }

        private bool CanUnshelve(ObservableCollection<object> shelvesetModels)
        {
            return shelvesetModels != null && shelvesetModels.Count == 1;
        }

        #endregion

        #region Details Command

        public RelayCommand<ObservableCollection<object>> DetailsCommand { get; private set; }

        private void Details(ObservableCollection<object> shelvesetModels)
        {
            var shelvesetModel = (ShelvesetModel) shelvesetModels.First();
           
            Messenger.Default.Send(new ShowUnshelveDetailsDialogMessage
                {
                    UnshelveDetailsServiceModel = new UnshelveDetailsServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, shelvesetModel.Shelveset)
                });
        }

        private bool CanDetails(ObservableCollection<object> shelvesetModels)
        {
            return shelvesetModels != null && shelvesetModels.Count == 1;
        }

        #endregion

        #region Delete Command

        public RelayCommand<ObservableCollection<object>> DeleteCommand { get; private set; }

        private void Delete(ObservableCollection<object> shelvesetModels)
        {
            var messageBoxResult = MessageBox.Show("Delete Shelveset\r\n\r\nAre you sure you want to delete the selected item? This operation is permanent.", "Team Pilgrim", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (messageBoxResult != MessageBoxResult.Yes) return;

            foreach (var shelvesetModel in shelvesetModels.Cast<ShelvesetModel>())
            {
                teamPilgrimServiceModelProvider.TryWorkspaceDeleteShelveset(ProjectCollectionServiceModel.TfsTeamProjectCollection, shelvesetModel.Shelveset.Name, shelvesetModel.Shelveset.OwnerName);
            }

            FindShelvesetsCommand.Execute(null);
        }

        private bool CanDelete(ObservableCollection<object> shelvesetModels)
        {
            return shelvesetModels != null && shelvesetModels.Any();
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

    }
}
