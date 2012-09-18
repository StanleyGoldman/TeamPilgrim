using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimModel : BaseModel
    {
        private readonly IPilgrimModelProvider _pilgrimModelProvider;

        public PilgrimModel(IPilgrimModelProvider pilgrimModelProvider)
        {
            _pilgrimModelProvider = pilgrimModelProvider;
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

        private PilgrimProjectCollectionModel[] _collections = new PilgrimProjectCollectionModel[0];

        public PilgrimProjectCollectionModel[] CollectionModels
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
            PilgrimProjectCollection[] fetchedCollections;
            if (_pilgrimModelProvider.TryGetCollections(out fetchedCollections))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    CollectionModels = fetchedCollections.Select(collection => new PilgrimProjectCollectionModel(_pilgrimModelProvider, collection)).ToArray();
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
