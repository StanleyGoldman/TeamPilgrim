using System;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectModel : BaseModel
    {
        private readonly IPilgrimModelProvider _pilgrimModelProvider;
        private readonly Uri _tpcUri;

        public PilgrimProjectModel(IPilgrimModelProvider pilgrimModelProvider, Uri tpcUri)
        {
            _pilgrimModelProvider = pilgrimModelProvider;
            _tpcUri = tpcUri;
            State = ModelStateEnum.Invalid;
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PilgrimProjectCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        #region Projects

        private PilgrimProject[] _projects = new PilgrimProject[0];

        public PilgrimProject[] Projects
        {
            get
            {
                VerifyCalledOnUiThread();
                return _projects;
            }
            private set
            {
                VerifyCalledOnUiThread();
                if (_projects == value) return;

                _projects = value;
                SendPropertyChanged("Projects");
            }
        }

        private void PilgrimProjectCallback(object state)
        {
            PilgrimProject[] fetchedProjects;
            if (_pilgrimModelProvider.TryGetProjects(out fetchedProjects, _tpcUri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        Projects = fetchedProjects;
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

        #endregion
    }
}