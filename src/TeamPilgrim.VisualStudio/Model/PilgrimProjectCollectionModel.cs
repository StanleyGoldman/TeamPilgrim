using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectCollectionModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly PilgrimProjectCollection _collection;

        public PilgrimProjectCollectionModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, PilgrimProjectCollection collection)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _collection = collection;

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

        public string Name
        {
            get { return _collection.ProjectCollection.Name; }
        }

        public Uri Uri
        {
            get { return _collection.ProjectCollection.Uri; }
        }

        public bool Offline
        {
            get { return _collection.ProjectCollection.Offline; }
        }

        public bool AutoReconnect
        {
            get { return _collection.ProjectCollection.AutoReconnect; }
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
                            .Select(project => new PilgrimProjectModel(_pilgrimServiceModelProvider, projectCollectionModel._collection, project))
                            .ToArray();

                        ProjectModels = pilgrimProjectModels;

                        BuildModel = new PilgrimBuildModel(_pilgrimServiceModelProvider, buildServiceModelProvider, projectCollectionModel._collection);

                        State = ModelStateEnum.Active;

                        foreach (var pilgrimProjectModel in pilgrimProjectModels)
                        {
                            pilgrimProjectModel.Activate();
                        }

                        BuildModel.Activate();
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

        #region BuildModel

        private PilgrimBuildModel _buildModel;

        public PilgrimBuildModel BuildModel
        {
            get
            {
                VerifyCalledOnUiThread();
                return _buildModel;
            }
            set
            {
                VerifyCalledOnUiThread();
                if (_buildModel == value) return;

                _buildModel = value;
                SendPropertyChanged("BuildModel");
            }
        }

        #endregion

    }
}