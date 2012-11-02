using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.TeamFoundation;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class TeamPilgrimModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TeamPilgrimModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            CollectionModels = new ObservableCollection<ProjectCollectionModel>();

            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;

            TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContextChangedEvent += TeamPilgrimPackageOnActiveProjectContextChangedEvent;

            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            TfsConnectCommand = new RelayCommand(TfsConnect, CanTfsConnect);
        }

        private void TeamPilgrimPackageOnActiveProjectContextChangedEvent(ProjectContextExt projectContext)
        {
            Task.Run(() => PopulatePilgrimModel());
        }

        private void PopulatePilgrimModel()
        {
            TfsTeamProjectCollection collection;

            Uri tpcAddress = null;
            if (TeamPilgrimPackage.TeamPilgrimVsService != null &&
                TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext != null &&
                TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext.DomainUri != null)
            {
                tpcAddress = new Uri(TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext.DomainUri);
            }

            if (_pilgrimServiceModelProvider.TryGetCollection(out collection, tpcAddress))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        CollectionModels.Clear();
                        if (collection != null)
                        {
                            CollectionModels.Add(new ProjectCollectionModel(collection, this, _pilgrimServiceModelProvider));
                        }
                    }));
            }
        }

        #region Refresh

        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand TfsConnectCommand { get; private set; }

        private void Refresh()
        {
            Task.Run(() => PopulatePilgrimModel());
        }

        private bool CanRefresh()
        {
            return true;
        }

        #endregion

        #region TFS Connect

        private void TfsConnect()
        {
            // TODO: This doesn't work as expected, there's probably a better way to do this
            EnvDTE.DTE dte = (EnvDTE.DTE)Marshal.GetActiveObject("VisualStudio.DTE.11.0");
            dte.ExecuteCommand("Team.ConnecttoTeamFoundationServer");
        }

        private bool CanTfsConnect()
        {
            return true;
        }

        #endregion

        #region CollectionModels

        public ObservableCollection<ProjectCollectionModel> CollectionModels { get; private set; }

        #endregion
    }
}
