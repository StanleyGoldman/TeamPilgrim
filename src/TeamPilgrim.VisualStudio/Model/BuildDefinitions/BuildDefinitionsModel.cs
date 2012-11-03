using System;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.BuildDefinitions
{
    public class BuildDefinitionsModel : BaseModel, IBuildDefinitionCommandModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly TfsTeamProjectCollection _collection;
        private readonly Project _project;

        public BuildDefinitionsModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection collection, Project project)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _collection = collection;
            _project = project;

            OpenBuildDefintionCommand = new RelayCommand<BuildDefinitionModel>(OpenBuildDefinition, CanOpenBuildDefinition);
            ViewBuildsCommand = new RelayCommand<BuildDefinitionModel>(ViewBuilds, CanViewBuilds);
            QueueBuildCommand = new RelayCommand<BuildDefinitionModel>(QueueBuild, CanQueueBuild);
            NewBuildDefinitionCommand = new RelayCommand(NewBuildDefinition, CanNewBuildDefinition);
            ManageBuildControllersCommand = new RelayCommand(ManageBuildControllers, CanManageBuildControllers);
            ManageBuildQualitiesCommand = new RelayCommand(ManageBuildQualities, CanManageBuildQualities);
            ManageBuildSecurityCommand = new RelayCommand(ManageBuildSecurity, CanManageBuildSecurity);

            IBuildDefinition[] buildDefinitions;
            if (_pilgrimServiceModelProvider.TryGetBuildDefinitionsByProjectName(out buildDefinitions, _collection, _project.Name))
            {
                BuildDefinitions = buildDefinitions
                    .Select(definition => new BuildDefinitionModel(this, definition))
                    .ToArray();
            }
        }

        #region OpenBuildDefinition Command

        public RelayCommand NewBuildDefinitionCommand { get; set; }

        private bool CanNewBuildDefinition()
        {
            return true;
        }

        private void NewBuildDefinition()
        {
            TeamPilgrimPackage.TeamPilgrimVsService.NewBuildDefinition(_project.Name);
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
            TeamPilgrimPackage.TeamPilgrimVsService.OpenBuildSecurityDialog(_project.Name, _project.Uri.ToString());
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
            TeamPilgrimPackage.TeamPilgrimVsService.OpenControllerAgentManager(_project.Name);
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
            TeamPilgrimPackage.TeamPilgrimVsService.OpenQualityManager(_project.Name);
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
            TeamPilgrimPackage.TeamPilgrimVsService.OpenBuildDefinition(buildDefinitionModel.Definition.Uri);
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

            TeamPilgrimPackage.TeamPilgrimVsService.ViewBuilds(_project.Name, buildDefinitionName, String.Empty, DateFilter.Today);
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

            TeamPilgrimPackage.TeamPilgrimVsService.QueueBuild(_project.Name, definitionModel.Definition.Uri);
        }

        #endregion

        #region BuildModel

        private BuildDefinitionModel[] _buildDefinitions;

        public BuildDefinitionModel[] BuildDefinitions
        {
            get
            {
                return _buildDefinitions;
            }
            set
            {
                if (_buildDefinitions == value) return;

                _buildDefinitions = value;
                SendPropertyChanged("BuildDefinitions");
            }
        }

        #endregion
    }
}
