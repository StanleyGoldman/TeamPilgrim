using System.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectModel : BaseModel
    {
        private readonly IPilgrimModelProvider _pilgrimModelProvider;
        private readonly PilgrimProjectCollection _collection;
        private readonly PilgrimProject _pilgrimProject;

        public PilgrimProjectModel(IPilgrimModelProvider pilgrimModelProvider, PilgrimProjectCollection collection, PilgrimProject pilgrimProject)
        {
            _pilgrimModelProvider = pilgrimModelProvider;
            _collection = collection;
            _pilgrimProject = pilgrimProject;
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PilgrimProjectCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        public string Name
        {
            get { return _pilgrimProject.Project.Name; }
        }

        private void PilgrimProjectCallback(object state)
        {
            
        }
    }
}
