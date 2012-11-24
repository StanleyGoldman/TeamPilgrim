using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer.BuildDefinitions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer
{
    public class ProjectServiceModel : BaseServiceModel
    {
        public TfsTeamProjectCollection TfsTeamProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public TeamPilgrimServiceModel TeamPilgrimServiceModel { get; private set; }

        public BuildDefinitionsServiceModel BuildDefinitionsServiceModel { get; private set; }
        
        public WorkItemQueryServiceModel WorkItemQueryServiceModel { get; private set; }

        public ObservableCollection<BaseModel> ChildObjects { get; private set; }

        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                SendPropertyChanged("IsActive");
            }
        }

        public ProjectServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimServiceModel teamPilgrimServiceModel, TfsTeamProjectCollection tfsTeamProjectCollection, Project project)
            :base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            TeamPilgrimServiceModel = teamPilgrimServiceModel;
            TfsTeamProjectCollection = tfsTeamProjectCollection;
            Project = project;

            ShowProjectAlertsCommand = new RelayCommand(ShowProjectAlerts, CanShowProjectAlerts);
            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);
            OpenPortalSettingsCommand = new RelayCommand(OpenPortalSettings, CanOpenPortalSettings);
            OpenSourceControlSettingsCommand = new RelayCommand(OpenSourceControlSettings, CanOpenSourceControlSettings);
            OpenAreasAndIterationsCommand = new RelayCommand(OpenAreasAndIterations, CanOpenAreasAndIterations);
            ShowSecuritySettingsCommand = new RelayCommand(ShowSecuritySettings, CanShowSecuritySettings);
            OpenGroupMembershipCommand = new RelayCommand(OpenGroupMembership, CanOpenGroupMembership);

            BuildDefinitionsServiceModel = new BuildDefinitionsServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, tfsTeamProjectCollection, project);

            WorkItemQueryServiceModel = new WorkItemQueryServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, tfsTeamProjectCollection, project);

            ChildObjects = new ObservableCollection<BaseModel>
                {
                    WorkItemQueryServiceModel, 
                    new ReportsModel(),
                    BuildDefinitionsServiceModel,
                    new TeamMembersModel(),
                    new SourceControlModel()
                };
            
            IsActive = teamPilgrimVsService.ActiveProjectContext.ProjectName == Project.Name;
        }

        #region Refresh Command

        protected override void Refresh()
        {
            WorkItemQueryServiceModel.RefreshCommand.Execute(null);
            BuildDefinitionsServiceModel.RefreshCommand.Execute(null);
        }

        protected override bool CanRefresh()
        {
            return true;
        }

        #endregion

        #region ShowProjectAlerts Command

        public RelayCommand ShowProjectAlertsCommand { get; private set; }

        private void ShowProjectAlerts()
        {
            CommandHandlerPackageWrapper.OpenProjectAlerts(TfsTeamProjectCollection, Project.Name);
        }

        private bool CanShowProjectAlerts()
        {
            return true;
        }

        #endregion

        #region OpenGroupMembership Command

        public RelayCommand OpenGroupMembershipCommand { get; private set; }

        private void OpenGroupMembership()
        {
            CommandHandlerPackageWrapper.OpenGroupMembership(TfsTeamProjectCollection, Project.Name, Project.Uri.ToString());
        }

        private bool CanOpenGroupMembership()
        {
            return true;
        }

        #endregion

        #region OpenAreasAndIterations Command

        public RelayCommand OpenAreasAndIterationsCommand { get; private set; }

        private void OpenAreasAndIterations()
        {
            teamPilgrimVsService.ShowWorkItemsAreasAndIterationsDialog(TfsTeamProjectCollection, Project.Name, Project.Uri.ToString());
        }

        private bool CanOpenAreasAndIterations()
        {
            return true;
        }

        #endregion

        #region OpenPortalSettings Command

        public RelayCommand OpenPortalSettingsCommand { get; private set; }

        private void OpenPortalSettings()
        {
            teamPilgrimVsService.ShowPortalSettings(TfsTeamProjectCollection, Project.Name, Project.Uri.ToString());
        }

        private bool CanOpenPortalSettings()
        {
            return true;
        }

        #endregion

        #region OpenSourceControlSettings Command

        public RelayCommand OpenSourceControlSettingsCommand { get; private set; }

        private void OpenSourceControlSettings()
        {
            teamPilgrimVsService.ShowSourceControlSettings();
        }

        private bool CanOpenSourceControlSettings()
        {
            return true;
        }

        #endregion

        #region ShowSecuritySettings Command

        public RelayCommand ShowSecuritySettingsCommand { get; private set; }

        private void ShowSecuritySettings()
        {
            CommandHandlerPackageWrapper.OpenSecuritySettings(TfsTeamProjectCollection, Project.Name, Project.Uri.ToString());
        }

        private bool CanShowSecuritySettings()
        {
            return true;
        }

        #endregion

        #region OpenSourceControl Command

        public RelayCommand OpenSourceControlCommand { get; private set; }

        private void OpenSourceControl()
        {
            teamPilgrimVsService.OpenSourceControl(Project.Name);
        }

        private bool CanOpenSourceControl()
        {
            return true;
        }

        #endregion
    }
}
