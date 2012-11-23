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
        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public TeamPilgrimServiceModel TeamPilgrimServiceModel { get; private set; }

        public BuildDefinitionsServiceModel BuildDefinitionsServiceModel { get; private set; }
        
        public WorkItemQueryServiceModel WorkItemQueryServiceModel { get; private set; }

        public ObservableCollection<BaseModel> ChildObjects { get; private set; }

        public ProjectServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimServiceModel teamPilgrimServiceModel, TfsTeamProjectCollection projectCollection, Project project)
            :base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            TeamPilgrimServiceModel = teamPilgrimServiceModel;
            ProjectCollection = projectCollection;
            Project = project;

            ShowProjectAlertsCommand = new RelayCommand(ShowProjectAlerts, CanShowProjectAlerts);
            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);
            OpenAreasAndIterationsCommand = new RelayCommand(OpenAreasAndIterations, CanOpenAreasAndIterations);
            ShowSecuritySettingsCommand = new RelayCommand(ShowSecuritySettings, CanShowSecuritySettings);
            OpenGroupMembershipCommand = new RelayCommand(OpenGroupMembership, CanOpenGroupMembership);

            BuildDefinitionsServiceModel = new BuildDefinitionsServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, projectCollection, project);

            WorkItemQueryServiceModel = new WorkItemQueryServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, projectCollection, project);

            ChildObjects = new ObservableCollection<BaseModel>
                {
                    WorkItemQueryServiceModel, 
                    new ReportsModel(),
                    BuildDefinitionsServiceModel,
                    new TeamMembersModel(),
                    new SourceControlModel()
                };
        }

        #region ShowProjectAlerts Command

        public RelayCommand ShowProjectAlertsCommand { get; private set; }

        private void ShowProjectAlerts()
        {
            CommandHandlerPackageWrapper.OpenProjectAlerts(ProjectCollection, Project.Name);
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
            CommandHandlerPackageWrapper.OpenGroupMembership(ProjectCollection, Project.Name, Project.Uri.ToString());
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
            teamPilgrimVsService.ShowWorkItemsAreasAndIterationsDialog(ProjectCollection, Project.Name, Project.Uri.ToString());
        }

        private bool CanOpenAreasAndIterations()
        {
            return true;
        }

        #endregion

        #region ShowSecuritySettings Command

        public RelayCommand ShowSecuritySettingsCommand { get; private set; }

        private void ShowSecuritySettings()
        {
            CommandHandlerPackageWrapper.OpenSecuritySettings(ProjectCollection, Project.Name, Project.Uri.ToString());
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
