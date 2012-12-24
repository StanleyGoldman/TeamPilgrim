using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class UnshelveChangesServiceModel : BaseServiceModel
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

        public UnshelveChangesServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider,
                                           ITeamPilgrimVsService teamPilgrimVsService,
                                           ProjectCollectionServiceModel projectCollectionServiceModel, WorkspaceServiceModel workspaceServiceModel)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectCollectionServiceModel = projectCollectionServiceModel;
            WorkspaceServiceModel = workspaceServiceModel;

            ShelvesetOwner = projectCollectionServiceModel.TfsTeamProjectCollection.AuthorizedIdentity.UniqueName;

            FindShelvesetsCommand = new RelayCommand<string>(FindShelvesets, CanFindShelvesets);
            ViewPendingSetCommand = new RelayCommand<ShelvesetModel>(ViewPendingSet, CanViewPendingSet);
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

    }
}
