using System;
using System.Threading;
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

        private void PilgrimProjectCollectionCallback(object state)
        {
        }
    }
}