using System.Collections.Generic;
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

        private Dictionary<string, PilgrimProjectModel> _projectModelDictionary;

        public PilgrimProjectCollectionModel(IPilgrimModelProvider pilgrimModelProvider)
        {
            _pilgrimModelProvider = pilgrimModelProvider;
            _projectModelDictionary = new Dictionary<string, PilgrimProjectModel>();

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

        private PilgrimProjectCollection[] _collections = new PilgrimProjectCollection[0];

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
