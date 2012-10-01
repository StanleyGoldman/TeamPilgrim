using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectCollectionModel : BaseModel
    {
        private readonly PilgrimModel _pilgrimModel;
        private readonly PilgrimProjectCollection _pilgrimProjectCollection;
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public PilgrimProjectCollectionModel(PilgrimProjectCollection pilgrimProjectCollection, PilgrimModel pilgrimModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider)
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

        public PilgrimProjectCollection PilgrimProjectCollection
        {
            get { return _pilgrimProjectCollection; }
        }

        public string Name
        {
            get { return PilgrimProjectCollection.ProjectCollection.Name; }
        }

        public Uri Uri
        {
            get { return PilgrimProjectCollection.ProjectCollection.Uri; }
        }

        public bool Offline
        {
            get { return PilgrimProjectCollection.ProjectCollection.Offline; }
        }

        public bool AutoReconnect
        {
            get { return PilgrimProjectCollection.ProjectCollection.AutoReconnect; }
        }

        private void PopulatePilgrimProjectCollectionModelCallback(object state)
        {
            PilgrimProject[] projects;
            IPilgrimBuildServiceModelProvider buildServiceModelProvider;
            
            var projectCollectionModel = this;

            if (_pilgrimServiceModelProvider.TryGetProjectsAndBuildServiceProvider(out projects, out buildServiceModelProvider, Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        var pilgrimProjectModels = projects
                            .Select(project => new PilgrimProjectModel(_pilgrimServiceModelProvider, projectCollectionModel.PilgrimProjectCollection, project, new PilgrimProjectBuildModel(_pilgrimServiceModelProvider, buildServiceModelProvider, projectCollectionModel.PilgrimProjectCollection, project)))
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