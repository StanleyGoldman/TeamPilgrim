using System;
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

            IBuildDefinition[] buildDefinitions;
            if (_pilgrimServiceModelProvider.TryGetBuildDefinitionsByProjectName(out buildDefinitions, _collection, _project.Name))
            {
                BuildDefinitions = buildDefinitions
                    .Select(definition => new BuildDefinitionModel(this, definition))
                    .ToArray();
            }
        }

        #region OpenBuildDefinition Command

        public RelayCommand<BuildDefinitionModel> OpenBuildDefintionCommand { get; set; }

        private bool CanOpenBuildDefinition(BuildDefinitionModel buildDefinitionWrapper)
        {
            return true;
        }

        private void OpenBuildDefinition(BuildDefinitionModel buildDefinitionWrapper)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.OpenBuildDefinition(buildDefinitionWrapper.Definition.Uri);
        }

        #endregion

        #region ViewBuilds Command

        public RelayCommand<BuildDefinitionModel> ViewBuildsCommand { get; set; }

        private bool CanViewBuilds(BuildDefinitionModel buildDefinitionWrapper)
        {
            return true;
        }

        private void ViewBuilds(BuildDefinitionModel buildDefinitionWrapper)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.ViewBuilds(_project.Name, buildDefinitionWrapper.Definition.Name, String.Empty, DateFilter.Today);
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
