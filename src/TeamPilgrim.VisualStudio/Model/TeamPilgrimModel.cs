using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class TeamPilgrimModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TeamPilgrimModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;

            State = ModelStateEnum.Invalid;

            TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContextChangedEvent += TeamPilgrimPackageOnActiveProjectContextChangedEvent;
        }

        private void TeamPilgrimPackageOnActiveProjectContextChangedEvent(ProjectContextExt projectContext)
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PilgrimModelCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();
        }

        #region Collections

        private ProjectCollectionModel[] _collections = new ProjectCollectionModel[0];

        public ProjectCollectionModel[] CollectionModels
        {
            get
            {
                VerifyCalledOnUiThread();
                return _collections;
            }
            private set
            {
                VerifyCalledOnUiThread();
                if (_collections == value) return;

                _collections = value;
                SendPropertyChanged("CollectionModels");
            }
        }

        private void PilgrimModelCallback(object state)
        {
            TfsTeamProjectCollection collection;

            Uri tpcAddress = null;
            if (TeamPilgrimPackage.TeamPilgrimVsService != null &&
                TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext.DomainUri != null)
            {
                tpcAddress = new Uri(TeamPilgrimPackage.TeamPilgrimVsService.ActiveProjectContext.DomainUri);
            }

            if (_pilgrimServiceModelProvider.TryGetCollection(out collection, tpcAddress))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        if (collection == null)
                        {
                            CollectionModels = new ProjectCollectionModel[0];
                            State = ModelStateEnum.Active;
                        }
                        else
                        {
                            var pilgrimProjectCollectionModel = new ProjectCollectionModel(collection, this,
                                                                                           _pilgrimServiceModelProvider);

                            CollectionModels = new[] { pilgrimProjectCollectionModel };
                            State = ModelStateEnum.Active;

                            pilgrimProjectCollectionModel.Activate();
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
