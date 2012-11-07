using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer.BuildDefinitions
{
    public class BuildDefinitionsModel : BaseModel, IBuildDefinitionCommandModel
    {
        public ObservableCollection<BuildDefinitionModel> BuildDefinitions { get; private set; }

        private readonly ITeamPilgrimServiceModelProvider _teamPilgrimServiceModelProvider;
        private readonly TfsTeamProjectCollection _collection;
        private readonly Project _project;

        public BuildDefinitionsModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TfsTeamProjectCollection collection, Project project)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            BuildDefinitions = new ObservableCollection<BuildDefinitionModel>();

            _teamPilgrimServiceModelProvider = teamPilgrimServiceModelProvider;
            _collection = collection;
            _project = project;

            OpenBuildDefintionCommand = new RelayCommand<BuildDefinitionModel>(OpenBuildDefinition, CanOpenBuildDefinition);
            ViewBuildsCommand = new RelayCommand<BuildDefinitionModel>(ViewBuilds, CanViewBuilds);
            DeleteBuildDefinitionCommand = new RelayCommand<BuildDefinitionModel>(DeleteBuildDefinition, CanDeleteBuildDefinition);
            CloneBuildDefinitionCommand = new RelayCommand<BuildDefinitionModel>(CloneBuildDefinition, CanCloneBuildDefinition);
            QueueBuildCommand = new RelayCommand<BuildDefinitionModel>(QueueBuild, CanQueueBuild);
            OpenProcessFileLocationCommand = new RelayCommand<BuildDefinitionModel>(OpenProcessFileLocation, CanOpenProcessFileLocation);
            ManageBuildDefinitionSecurityCommand = new RelayCommand<BuildDefinitionModel>(ManageBuildDefinitionSecurity, CanManageBuildDefinitionSecurity);

            NewBuildDefinitionCommand = new RelayCommand(NewBuildDefinition, CanNewBuildDefinition);
            ManageBuildControllersCommand = new RelayCommand(ManageBuildControllers, CanManageBuildControllers);
            ManageBuildQualitiesCommand = new RelayCommand(ManageBuildQualities, CanManageBuildQualities);
            ManageBuildSecurityCommand = new RelayCommand(ManageBuildSecurity, CanManageBuildSecurity);

            IBuildDefinition[] buildDefinitions;
            if (_teamPilgrimServiceModelProvider.TryGetBuildDefinitionsByProjectName(out buildDefinitions, _collection, _project.Name))
            {
                foreach (var buildDefinitionModel in buildDefinitions.Select(definition => new BuildDefinitionModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, this, definition)))
                {
                    BuildDefinitions.Add(buildDefinitionModel);
                }
            }
        }

        #region NewBuildDefinition Command

        public RelayCommand NewBuildDefinitionCommand { get; set; }

        private bool CanNewBuildDefinition()
        {
            return true;
        }

        private void NewBuildDefinition()
        {
            teamPilgrimVsService.NewBuildDefinition(_project.Name);
        }

        #endregion

        #region ManageBuildSecurity Command

        public RelayCommand ManageBuildSecurityCommand { get; set; }

        private bool CanManageBuildSecurity()
        {
            return true;
        }

        private void ManageBuildSecurity()
        {
            teamPilgrimVsService.OpenBuildSecurityDialog(_project.Name, _project.Uri.ToString());
        }

        #endregion

        #region ManageBuildControllers Command

        public RelayCommand ManageBuildControllersCommand { get; set; }

        private bool CanManageBuildControllers()
        {
            return true;
        }

        private void ManageBuildControllers()
        {
            teamPilgrimVsService.OpenControllerAgentManager(_project.Name);
        }

        #endregion

        #region ManageBuildQualities Command

        public RelayCommand ManageBuildQualitiesCommand { get; set; }

        private bool CanManageBuildQualities()
        {
            return true;
        }

        private void ManageBuildQualities()
        {
            teamPilgrimVsService.OpenQualityManager(_project.Name);
        }

        #endregion

        #region DeleteBuildDefinition Command

        public RelayCommand<BuildDefinitionModel> DeleteBuildDefinitionCommand { get; set; }

        private bool CanDeleteBuildDefinition(BuildDefinitionModel buildDefinitionModel)
        {
            return true;
        }

        private void DeleteBuildDefinition(BuildDefinitionModel buildDefinitionModel)
        {
            if (_teamPilgrimServiceModelProvider.TryDeleteBuildDefinition(buildDefinitionModel.Definition))
            {
                BuildDefinitions.Remove(buildDefinitionModel);
            }
        }

        #endregion

        #region CloneBuildDefinition Command

        public RelayCommand<BuildDefinitionModel> CloneBuildDefinitionCommand { get; set; }

        private bool CanCloneBuildDefinition(BuildDefinitionModel buildDefinitionModel)
        {
            return true;
        }

        private void CloneBuildDefinition(BuildDefinitionModel buildDefinitionModel)
        {
            IBuildDefinition buildDefinition;
            if (_teamPilgrimServiceModelProvider.TryCloneQueryDefinition(out buildDefinition, _collection, _project, buildDefinitionModel.Definition))
            {
                BuildDefinitions.Add(new BuildDefinitionModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, this, buildDefinition));
            }
        }

        #endregion

        #region ManageBuildDefinitionSecurity Command

        public RelayCommand<BuildDefinitionModel> ManageBuildDefinitionSecurityCommand { get; set; }

        private bool CanManageBuildDefinitionSecurity(BuildDefinitionModel buildDefinitionModel)
        {
            return true;
        }

        private void ManageBuildDefinitionSecurity(BuildDefinitionModel buildDefinitionModel)
        {
            teamPilgrimVsService.OpenBuildDefinitionSecurityDialog(_project.Name, _project.Uri.ToString(), buildDefinitionModel.Definition.Name, buildDefinitionModel.Definition.Uri.ToString());
        }

        #endregion

        #region OpenBuildDefinition Command

        public RelayCommand<BuildDefinitionModel> OpenBuildDefintionCommand { get; set; }

        private bool CanOpenBuildDefinition(BuildDefinitionModel buildDefinitionModel)
        {
            return true;
        }

        private void OpenBuildDefinition(BuildDefinitionModel buildDefinitionModel)
        {
            teamPilgrimVsService.OpenBuildDefinition(buildDefinitionModel.Definition.Uri);
        }

        #endregion

        #region OpenProcessFileLocation Command

        public RelayCommand<BuildDefinitionModel> OpenProcessFileLocationCommand { get; set; }

        private bool CanOpenProcessFileLocation(BuildDefinitionModel buildDefinitionModel)
        {
            return true;
        }

        private void OpenProcessFileLocation(BuildDefinitionModel buildDefinitionModel)
        {
            teamPilgrimVsService.OpenProcessFileLocation(buildDefinitionModel.Definition.Uri);
        }

        #endregion

        #region ViewBuilds Command

        public RelayCommand<BuildDefinitionModel> ViewBuildsCommand { get; set; }

        private bool CanViewBuilds(BuildDefinitionModel buildDefinitionWrapper)
        {
            return true;
        }

        private void ViewBuilds(BuildDefinitionModel buildDefinitionModel)
        {
            string buildDefinitionName = null;
            if (buildDefinitionModel != null)
            {
                buildDefinitionName = buildDefinitionModel.Definition.Name;
            }

            teamPilgrimVsService.ViewBuilds(_project.Name, buildDefinitionName, String.Empty, DateFilter.Today);
        }

        #endregion

        #region QueueBuild Command

        public RelayCommand<BuildDefinitionModel> QueueBuildCommand { get; set; }

        private bool CanQueueBuild(BuildDefinitionModel buildDefinitionWrapper)
        {
            return true;
        }

        private void QueueBuild(BuildDefinitionModel buildDefinitionModel)
        {
            var definitionModel = buildDefinitionModel ?? BuildDefinitions.FirstOrDefault();

            //TODO: SG 11/3/2012 Fix this, I know this cannot be true
            Debug.Assert(definitionModel != null, "definitionModel != null");

            teamPilgrimVsService.QueueBuild(_project.Name, definitionModel.Definition.Uri);
        }

        #endregion
    }
}
