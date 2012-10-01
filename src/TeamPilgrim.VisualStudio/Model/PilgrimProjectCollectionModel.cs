using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectCollectionModel : BaseModel
    {
        public PilgrimModel PilgrimModel { get; private set; }
     
        public TfsTeamProjectCollection TfsTeamProjectCollection { get; private set; }

        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public PilgrimProjectCollectionModel(TfsTeamProjectCollection pilgrimProjectCollection, PilgrimModel pilgrimModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            TfsTeamProjectCollection = pilgrimProjectCollection;
            PilgrimModel = pilgrimModel;

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
            IPilgrimBuildServiceModelProvider buildServiceModelProvider;
            
            var projectCollectionModel = this;

            if (_pilgrimServiceModelProvider.TryGetProjectsAndBuildServiceProvider(out projects, out buildServiceModelProvider, TfsTeamProjectCollection.Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        var pilgrimProjectModels = projects
                            .Select(project => new PilgrimProjectModel(_pilgrimServiceModelProvider, projectCollectionModel.TfsTeamProjectCollection, project, new PilgrimProjectBuildModel(_pilgrimServiceModelProvider, buildServiceModelProvider, projectCollectionModel.TfsTeamProjectCollection, project)))
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

        private PilgrimProjectModel[] _projectModels = new PilgrimProjectModel[0];

        public PilgrimProjectModel[] ProjectModels
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