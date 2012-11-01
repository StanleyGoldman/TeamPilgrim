using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class ProjectCollectionModel : BaseModel
    {
        public TeamPilgrimModel TeamPilgrimModel { get; private set; }
     
        public TfsTeamProjectCollection TfsTeamProjectCollection { get; private set; }

        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public ProjectCollectionModel(TfsTeamProjectCollection pilgrimProjectCollection, TeamPilgrimModel teamPilgrimModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            TfsTeamProjectCollection = pilgrimProjectCollection;
            TeamPilgrimModel = teamPilgrimModel;

            State = ModelStateEnum.Invalid;
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PopulatePilgrimProjectCollectionModelCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        private void PopulatePilgrimProjectCollectionModelCallback(object state)
        {
            Project[] projects;
            
            var projectCollectionModel = this;

            if (_pilgrimServiceModelProvider.TryGetProjects(out projects, TfsTeamProjectCollection.Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        var pilgrimProjectModels = projects
                            .Select(project => new ProjectModel(_pilgrimServiceModelProvider, projectCollectionModel.TfsTeamProjectCollection, project, new ProjectBuildModel(_pilgrimServiceModelProvider, projectCollectionModel.TfsTeamProjectCollection, project)))
                            .ToArray();

                        ProjectModels = pilgrimProjectModels;

                        State = ModelStateEnum.Active;

                        foreach (var pilgrimProjectModel in pilgrimProjectModels)
                        {
                            pilgrimProjectModel.Activate();
                        }
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

        #region ProjectModels

        private ProjectModel[] _projectModels = new ProjectModel[0];

        public ProjectModel[] ProjectModels
        {
            get
            {
                VerifyCalledOnUiThread();
                return _projectModels;
            }
            private set
            {
                VerifyCalledOnUiThread();
                if (_projectModels == value) return;

                _projectModels = value;
                SendPropertyChanged("ProjectModels");
            }
        }

        #endregion
    }
}