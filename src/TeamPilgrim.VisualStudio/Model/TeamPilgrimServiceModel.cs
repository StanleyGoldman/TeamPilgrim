using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
    public class TeamPilgrimServiceModel : BaseServiceModel
    {
        private readonly ITeamPilgrimServiceModelProvider _teamPilgrimServiceModelProvider;
        private readonly ITeamPilgrimVsService _teamPilgrimVsService;

        public ObservableCollection<ProjectCollectionServiceModel> ProjectCollectionModels { get; private set; }
        public ObservableCollection<WorkspaceInfoModel> WorkspaceInfoModels { get; private set; }

        private ProjectCollectionServiceModel _activeProjectCollectionModel = null;

        public ProjectCollectionServiceModel ActiveProjectCollectionModel
        {
            get
            {
                return _activeProjectCollectionModel;
            }
            private set
            {
                if (_activeProjectCollectionModel == value) return;

                _activeProjectCollectionModel = value;

                SendPropertyChanged("ActiveProjectCollectionModel");
            }
        }

        private WorkspaceInfoModel _selectedWorkspaceInfoModel = null;

        public WorkspaceInfoModel SelectedWorkspaceInfoModel
        {
            get
            {
                return _selectedWorkspaceInfoModel;
            }
            private set
            {
                if (_selectedWorkspaceInfoModel == value) return;

                _selectedWorkspaceInfoModel = value;

                LoadWorkspaceModel(SelectedWorkspaceInfoModel);

                SendPropertyChanged("SelectedWorkspaceInfoModel");
            }
        }

        private WorkspaceServiceModel _selectedWorkspaceModel;

        public WorkspaceServiceModel SelectedWorkspaceModel
        {
            get
            {
                return _selectedWorkspaceModel;
            }
            private set
            {
                if (_selectedWorkspaceModel == value) return;

                _selectedWorkspaceModel = value;

                SendPropertyChanged("SelectedWorkspaceModel");
            }
        }

        public TeamPilgrimServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectCollectionModels = new ObservableCollection<ProjectCollectionServiceModel>();
            WorkspaceInfoModels = new ObservableCollection<WorkspaceInfoModel>();

            ProjectCollectionModels.CollectionChanged += ProjectCollectionModelsOnCollectionChanged;
            WorkspaceInfoModels.CollectionChanged += WorkspaceInfoModelsOnCollectionChanged;

            _teamPilgrimServiceModelProvider = teamPilgrimServiceModelProvider;
            _teamPilgrimVsService = teamPilgrimVsService;
            
            _teamPilgrimVsService.ContextChangedEvent += TeamPilgrimPackageOnContextChangedEvent;

            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            TfsConnectCommand = new RelayCommand(TfsConnect, CanTfsConnect);
            ShowResolveConflicttManagerCommand = new RelayCommand(ShowResolveConflicttManager, CanShowResolveConflicttManager);

            PopulatePilgrimModel();
        }

        private void ProjectCollectionModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            ActiveProjectCollectionModel = ProjectCollectionModels.FirstOrDefault();
        }

        private void WorkspaceInfoModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (WorkspaceInfoModels.Any())
            {
                if (SelectedWorkspaceInfoModel == null || !WorkspaceInfoModels.Contains(SelectedWorkspaceInfoModel))
                {
                    SelectedWorkspaceInfoModel = WorkspaceInfoModels.First();
                }
            }
            else
            {
                SelectedWorkspaceInfoModel = null;
            }
        }

        private void TeamPilgrimPackageOnContextChangedEvent(ProjectContextExt projectContext)
        {
            Task.Run(() => PopulatePilgrimModel());
        }

        private void PopulatePilgrimModel()
        {
            if (_teamPilgrimVsService.ActiveProjectContext == null ||
                _teamPilgrimVsService.ActiveProjectContext.DomainUri == null) return;

            var tpcAddress = new Uri(_teamPilgrimVsService.ActiveProjectContext.DomainUri);
            TfsTeamProjectCollection collection;

            if (_teamPilgrimServiceModelProvider.TryGetCollection(out collection, tpcAddress))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        ProjectCollectionModels.Clear();
                        if (collection != null)
                        {
                            ProjectCollectionModels.Add(new ProjectCollectionServiceModel(_teamPilgrimServiceModelProvider,
                                                                            _teamPilgrimVsService, this, collection));
                        }
                    }));
            }

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
            {
                WorkspaceInfo[] workspaceInfos;
                if (_teamPilgrimServiceModelProvider.TryGetLocalWorkspaceInfos(out workspaceInfos, collection.InstanceId))
                {
                    foreach (var workspaceInfo in workspaceInfos)
                    {
                        WorkspaceInfoModels.Add(new WorkspaceInfoModel(workspaceInfo));
                    }
                }
            }));
        }

        private void LoadWorkspaceModel(WorkspaceInfoModel selectedWorkspaceInfoModel)
        {
            Workspace workspace;
            var projectCollectionModel = ProjectCollectionModels[0];

            Debug.Assert(projectCollectionModel != null, "projectCollectionModel != null");

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    if (_teamPilgrimServiceModelProvider.TryGetWorkspace(out workspace, selectedWorkspaceInfoModel.WorkspaceInfo, projectCollectionModel.TfsTeamProjectCollection))
                    {
                        SelectedWorkspaceModel = new WorkspaceServiceModel(_teamPilgrimServiceModelProvider, teamPilgrimVsService, this.ActiveProjectCollectionModel, workspace);
                    }
                }));
        }

        #region Refresh Command

        public RelayCommand RefreshCommand { get; private set; }

        private void Refresh()
        {
            Task.Run(() => PopulatePilgrimModel());
        }

        private bool CanRefresh()
        {
            return true;
        }

        #endregion

        #region ShowResolveConflicttManager Command

        public RelayCommand ShowResolveConflicttManagerCommand { get; private set; }

        private void ShowResolveConflicttManager()
        {
            if (SelectedWorkspaceModel == null) 
                return;

            var paths = 
                SelectedWorkspaceModel.Workspace.Folders
                    .Select(folder => folder.ServerItem).ToArray();

            _teamPilgrimVsService.ResolveConflicts(SelectedWorkspaceModel.Workspace, paths, true, false);
        }

        private bool CanShowResolveConflicttManager()
        {
            return true;
        }

        #endregion

        #region TFSConnect Command

        public RelayCommand TfsConnectCommand { get; private set; }

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
