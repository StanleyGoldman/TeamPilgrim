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

            if (ThreadPool.QueueUserWorkItem(PilgrimCollectionCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        #region Collections

        private PilgrimProjectCollection[] _collections;

        public PilgrimProjectCollection[] Collections
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
                SendPropertyChanged("Collections");
            }
        }

        private void PilgrimCollectionCallback(object state)
        {
            PilgrimProjectCollection[] fetchedCollections;
            if (_pilgrimModelProvider.TryGetCollections(out fetchedCollections))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        Collections = fetchedCollections;
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
