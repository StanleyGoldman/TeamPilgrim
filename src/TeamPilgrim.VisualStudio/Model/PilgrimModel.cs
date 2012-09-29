using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly TestCommandModel _testCommand;

        public PilgrimModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _testCommand = new TestCommandModel(); 

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

        public TestCommandModel TestCommand
        {
            get { return _testCommand; }
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
            if (_pilgrimServiceModelProvider.TryGetCollections(out fetchedCollections))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    var pilgrimProjectCollectionModels = fetchedCollections.Select(collection => new PilgrimProjectCollectionModel(_pilgrimServiceModelProvider, collection)).ToArray();

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
