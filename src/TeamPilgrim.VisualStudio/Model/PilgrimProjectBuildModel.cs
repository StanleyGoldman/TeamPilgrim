using System;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectBuildModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly IPilgrimBuildServiceModelProvider _buildServiceModelProvider;
        private readonly TfsTeamProjectCollection _collection;
        private readonly Project _project;

        public PilgrimProjectBuildModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, IPilgrimBuildServiceModelProvider buildServiceModelProvider, TfsTeamProjectCollection collection, Project project)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _buildServiceModelProvider = buildServiceModelProvider;
            _collection = collection;
            _project = project;

            OpenBuildDefintionCommand = new RelayCommand<BuildDefinitionWrapper>(OpenBuildDefinition, CanOpenBuildDefinition);

            State = ModelStateEnum.Invalid;
        }

        #region OpenBuildDefinition Command

        private bool CanOpenBuildDefinition(BuildDefinitionWrapper buildDefinitionWrapper)
        {
            return true;
        }

        private void OpenBuildDefinition(BuildDefinitionWrapper buildDefinitionWrapper)
        {
            if (TeamPilgrimPackage.TeamFoundationBuild != null)
                TeamPilgrimPackage.TeamFoundationBuild.DetailsManager.OpenBuild(buildDefinitionWrapper.Uri);
        }

        #endregion

        public RelayCommand<BuildDefinitionWrapper> OpenBuildDefintionCommand { get; set; }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PopulatePilgrimBuildModelCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        private void PopulatePilgrimBuildModelCallback(object state)
        {
            BuildDefinitionWrapper[] buildDefinitions;
            if (_buildServiceModelProvider.TryGetBuildDefinitionsByProjectName(out buildDefinitions, _project.Name))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        BuildDefinitions = buildDefinitions;
                        State = ModelStateEnum.Active;
                    }));
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    State = ModelStateEnum.Invalid;
                }));
            }
        }

        #region BuildModel

        private BuildDefinitionWrapper[] _buildDefinitions;

        public BuildDefinitionWrapper[] BuildDefinitions
        {
            get
            {
                VerifyCalledOnUiThread();
                return _buildDefinitions;
            }
            set
            {
                VerifyCalledOnUiThread();
                if (_buildDefinitions == value) return;

                _buildDefinitions = value;
                SendPropertyChanged("BuildDefinitions");
            }
        }

        #endregion
    }
}
