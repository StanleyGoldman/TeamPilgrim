using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class TeamPilgrimModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TeamPilgrimModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;

            TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContextChangedEvent += TeamPilgrimPackageOnActiveProjectContextChangedEvent;

            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
        }

        private void TeamPilgrimPackageOnActiveProjectContextChangedEvent(ProjectContextExt projectContext)
        {
            ThreadPool.QueueUserWorkItem(PilgrimModelCallback);
        }

        private void PilgrimModelCallback(object state)
        {
            TfsTeamProjectCollection collection;

            Uri tpcAddress = null;
            if (TeamPilgrimPackage.TeamPilgrimVsService != null &&
                TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext.DomainUri != null)
            {
                tpcAddress = new Uri(TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext.DomainUri);
            }

            if (_pilgrimServiceModelProvider.TryGetCollection(out collection, tpcAddress))
            {
                if (collection == null)
                {
                    CollectionModels = new ProjectCollectionModel[0];
                }
                else
                {
                    var pilgrimProjectCollectionModel = new ProjectCollectionModel(collection, this, _pilgrimServiceModelProvider);
                    CollectionModels = new[] { pilgrimProjectCollectionModel };
                }
            }
        }

        #region Refresh

        public RelayCommand RefreshCommand { get; private set; }

        private void Refresh()
        {
            ThreadPool.QueueUserWorkItem(PilgrimModelCallback);
        }

        private bool CanRefresh()
        {
            return true;
        }

        #endregion

        #region Collections

        private ProjectCollectionModel[] _collections = new ProjectCollectionModel[0];

        public ProjectCollectionModel[] CollectionModels
        {
            get
            {
                return _collections;
            }
            private set
            {
                if (_collections == value) return;

                _collections = value;
                SendPropertyChanged("CollectionModels");
            }
        }

        #endregion
    }
}
