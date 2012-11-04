using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class TeamPilgrimModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly ITeamPilgrimVsService _teamPilgrimVsService;

        public ObservableCollection<ProjectCollectionModel> CollectionModels { get; private set; }
        public ObservableCollection<WorkspaceInfoModel> WorkspaceInfoModels { get; private set; }

        public TeamPilgrimModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            CollectionModels = new ObservableCollection<ProjectCollectionModel>();
            WorkspaceInfoModels = new ObservableCollection<WorkspaceInfoModel>();

            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _teamPilgrimVsService = teamPilgrimVsService;

            _teamPilgrimVsService.ActiveProjectContextChangedEvent += TeamPilgrimPackageOnActiveProjectContextChangedEvent;

            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            TfsConnectCommand = new RelayCommand(TfsConnect, CanTfsConnect);

            PopulatePilgrimModel();
        }

        private void TeamPilgrimPackageOnActiveProjectContextChangedEvent(ProjectContextExt projectContext)
        {
            Task.Run(() => PopulatePilgrimModel());
        }

        private void PopulatePilgrimModel()
        {
            if (_teamPilgrimVsService.ActiveProjectContext == null ||
                _teamPilgrimVsService.ActiveProjectContext.DomainUri == null) return;

            var tpcAddress = new Uri(_teamPilgrimVsService.ActiveProjectContext.DomainUri);
            TfsTeamProjectCollection collection;

            if (_pilgrimServiceModelProvider.TryGetCollection(out collection, tpcAddress))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        CollectionModels.Clear();
                        if (collection != null)
                        {
                            CollectionModels.Add(new ProjectCollectionModel(_pilgrimServiceModelProvider,
                                                                            _teamPilgrimVsService, this, collection));
                        }
                    }));
            }

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
            {
                WorkspaceInfo[] workspaceInfos;
                if (_pilgrimServiceModelProvider.TryGetLocalWorkspaceInfos(out workspaceInfos, collection.InstanceId))
                {
                    foreach (var workspaceInfo in workspaceInfos)
                    {
                        WorkspaceInfoModels.Add(new WorkspaceInfoModel(pilgrimServiceModelProvider, teamPilgrimVsService, workspaceInfo));
                    }
                }
            }));
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
            _teamPilgrimVsService.TfsConnect();
        }

        private bool CanTfsConnect()
        {
            return true;
        }

        #endregion
    }
}
