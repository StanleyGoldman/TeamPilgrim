using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectCollectionModel : BaseModel
    {
        private readonly IPilgrimModelProvider _pilgrimModelProvider;
        private readonly PilgrimProjectCollection _collection;

        public PilgrimProjectCollectionModel(IPilgrimModelProvider pilgrimModelProvider, PilgrimProjectCollection collection)
        {
            _pilgrimModelProvider = pilgrimModelProvider;
            _collection = collection;

            State = ModelStateEnum.Invalid;
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PilgrimProjectCollectionCallback))
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

        private void PilgrimProjectCollectionCallback(object state)
        {
            PilgrimProject[] projects;
            var blah = this;

            if (_pilgrimModelProvider.TryGetProjects(out projects, Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        var pilgrimProjectModels = projects.Select(project => new PilgrimProjectModel(_pilgrimModelProvider, blah._collection, project)).ToArray();

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

        #endregion

    }
}