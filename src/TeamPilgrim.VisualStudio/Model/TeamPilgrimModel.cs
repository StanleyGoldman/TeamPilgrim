using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Shell;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class TeamPilgrimModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TeamPilgrimModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;

            State = ModelStateEnum.Invalid;
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PilgrimModelCallback))
            {
                State = ModelStateEnum.Fetching;
            }
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
            TfsTeamProjectCollection[] fetchedCollections;
            if (_pilgrimServiceModelProvider.TryGetCollections(out fetchedCollections))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    var pilgrimProjectCollectionModels = fetchedCollections.Select(collection => new ProjectCollectionModel(collection, this, _pilgrimServiceModelProvider)).ToArray();

                    CollectionModels = pilgrimProjectCollectionModels;
                    State = ModelStateEnum.Active;
                    
                    foreach (var pilgrimProjectCollectionModel in pilgrimProjectCollectionModels)
                    {
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
