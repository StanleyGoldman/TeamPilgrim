using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class TeamPilgrimServiceModel : BaseServiceModel
    {
        private readonly ITeamPilgrimServiceModelProvider _teamPilgrimServiceModelProvider;
        private readonly ITeamPilgrimVsService _teamPilgrimVsService;

        public ObservableCollection<ProjectCollectionServiceModel> ProjectCollectionModels { get; private set; }
        public ObservableCollection<WorkspaceInfoModel> WorkspaceInfoModels { get; private set; }

        private bool _connecting;
        public bool Connecting
        {
            get
            {
                return _connecting;
            }
            private set
            {
                if (_connecting == value) return;

                _connecting = value;

                SendPropertyChanged("Connecting");
            }
        }

        private bool _solutionIsOpen;
        public bool SolutionIsOpen
        {
            get
            {
                return _solutionIsOpen;
            }
            private set
            {
                if (_solutionIsOpen == value) return;

                _solutionIsOpen = value;

                SendPropertyChanged("SolutionIsOpen");

                if (SelectedWorkspaceModel != null)
                    SelectedWorkspaceModel.RefreshPendingChangesCommand.Execute(null);
            }
        }

        private bool _filterSolution;
        public bool FilterSolution
        {
            get
            {
                return _filterSolution;
            }
            private set
            {
                if (_filterSolution == value) return;

                _filterSolution = value;

                SendPropertyChanged("FilterSolution");

                if (SelectedWorkspaceModel != null)
                    SelectedWorkspaceModel.RefreshPendingChangesCommand.Execute(null);
            }
        }

        private string _connectingServer;

        public string ConnectingServer
        {
            get
            {
                return _connectingServer;
            }
            private set
            {
                if (_connectingServer == value) return;

                _connectingServer = value;

                SendPropertyChanged("ConnectingServer");
            }
        }

        private ServerConnectedEventArgs.CompletionStatusEnum _connectedStatus;

        public ServerConnectedEventArgs.CompletionStatusEnum ConnectedStatus
        {
            get
            {
                return _connectedStatus;
            }
            private set
            {
                if (_connectedStatus == value) return;

                _connectedStatus = value;

                SendPropertyChanged("ConnectedStatus");
            }
        }

        private Exception _connectedError;

        public Exception ConnectedError
        {
            get
            {
                return _connectedError;
            }
            private set
            {
                if (_connectedError == value) return;

                _connectedError = value;

                SendPropertyChanged("ConnectedError");
            }
        }

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

                if (SelectedWorkspaceInfoModel != null) 
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

            _teamPilgrimVsService.TeamFoundationHost.ContextChanged += TeamFoundationHostOnContextChanged;
            _teamPilgrimVsService.TeamFoundationHost.ServerConnecting += delegate(object sender, ServerConnectingEventArgs args)
                {
                    Connecting = true;
                    ConnectingServer = args.TeamProjectCollection.Name;
                };

            _teamPilgrimVsService.TeamFoundationHost.ServerConnected += delegate(object sender, ServerConnectedEventArgs args)
                {
                    Connecting = false;
                    ConnectingServer = args.TeamProjectCollection.Name;
                    ConnectedError = args.Error;
                    ConnectedStatus = args.Status;
                };

            SolutionIsOpen = _teamPilgrimVsService.SolutionIsOpen;
            _teamPilgrimVsService.SolutionStateChanged += () =>
                {
                    SolutionIsOpen = _teamPilgrimVsService.SolutionIsOpen;
                };

            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            TfsConnectCommand = new RelayCommand(TfsConnect, CanTfsConnect);
            ShowResolveConflicttManagerCommand = new RelayCommand(ShowResolveConflicttManager, CanShowResolveConflicttManager);

            PopulateTeamPilgrimServiceModel();
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

        private void TeamFoundationHostOnContextChanged(object sender, ContextChangedEventArgs contextChangedEventArgs)
        {
            if (contextChangedEventArgs.TeamProjectCollectionChanged)
            {
                Task.Run(() => PopulateTeamPilgrimServiceModel());
            }
            else if (contextChangedEventArgs.TeamProjectChanged)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                    {
                        var activeProjectContext = _teamPilgrimVsService.ActiveProjectContext;
                        
                        foreach (var projectModel in ActiveProjectCollectionModel.ProjectModels)
                        {
                            projectModel.IsActive = projectModel.Project.Name == activeProjectContext.ProjectName;
                        }
                    }));
            }
        }

        private void PopulateTeamPilgrimServiceModel()
        {
            var activeProjectContext = _teamPilgrimVsService.ActiveProjectContext;

            if (activeProjectContext == null ||
                activeProjectContext.DomainUri == null)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    new ThreadStart(() => ProjectCollectionModels.Clear()));

                return;
            }

            var tpcAddress = new Uri(activeProjectContext.DomainUri);
            TfsTeamProjectCollection collection;

            if (_teamPilgrimServiceModelProvider.TryGetCollection(out collection, tpcAddress))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                    {
                        ProjectCollectionModels.Clear();
                        if (collection != null)
                        {
                            var projectCollectionServiceModel = new ProjectCollectionServiceModel(_teamPilgrimServiceModelProvider, _teamPilgrimVsService, this, collection);
                            ProjectCollectionModels.Add(projectCollectionServiceModel);
                        }
                    }));
            }

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
            {
                WorkspaceInfo[] workspaceInfos;
                if (_teamPilgrimServiceModelProvider.TryGetLocalWorkspaceInfos(out workspaceInfos, collection.InstanceId))
                {
                    WorkspaceInfoModels.Clear();

                    var activeWorkspace = _teamPilgrimVsService.ActiveWorkspace;

                    WorkspaceInfoModel selectedWorkspaceInfoModel = null;

                    foreach (var workspaceInfo in workspaceInfos)
                    {
                        var workspaceInfoModel = new WorkspaceInfoModel(workspaceInfo);
                        WorkspaceInfoModels.Add(workspaceInfoModel);

                        if (activeWorkspace != null && activeWorkspace.QualifiedName == workspaceInfo.QualifiedName)
                        {
                            selectedWorkspaceInfoModel = workspaceInfoModel;
                        }
                    }

                    if (selectedWorkspaceInfoModel != null)
                    {
                        SelectedWorkspaceInfoModel = selectedWorkspaceInfoModel;
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
            Task.Run(() => PopulateTeamPilgrimServiceModel());
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
