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
        private readonly PilgrimModel _pilgrimModel;
        private readonly TfsTeamProjectCollection _pilgrimProjectCollection;
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public PilgrimProjectCollectionModel(TfsTeamProjectCollection pilgrimProjectCollection, PilgrimModel pilgrimModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _pilgrimProjectCollection = pilgrimProjectCollection;
            _pilgrimModel = pilgrimModel;

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

        public PilgrimModel PilgrimModel
        {
            get { return _pilgrimModel; }
        }

        public TfsTeamProjectCollection TfsTeamProjectCollection
        {
            get { return _pilgrimProjectCollection; }
        }

        public string Name
        {
            get { return TfsTeamProjectCollection.Name; }
        }

        public Uri Uri
        {
            get { return TfsTeamProjectCollection.Uri; }
        }

        private void PopulatePilgrimProjectCollectionModelCallback(object state)
        {
            Project[] projects;
            IPilgrimBuildServiceModelProvider buildServiceModelProvider;
            
            var projectCollectionModel = this;

            if (_pilgrimServiceModelProvider.TryGetProjectsAndBuildServiceProvider(out projects, out buildServiceModelProvider, Uri))
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