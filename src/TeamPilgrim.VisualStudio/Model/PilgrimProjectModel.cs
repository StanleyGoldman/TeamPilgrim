using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Views;
using JustAProgrammer.TeamPilgrim.VisualStudio.Views.ProjectNodes;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectModel : BaseModel
    {
        private readonly IPilgrimModelProvider _pilgrimModelProvider;
        private readonly PilgrimProjectCollection _collection;
        private readonly PilgrimProject _pilgrimProject;
        private readonly ProjectNode[] _childObjects;

        public PilgrimProjectModel(IPilgrimModelProvider pilgrimModelProvider, PilgrimProjectCollection collection, PilgrimProject pilgrimProject)
        {
            _pilgrimModelProvider = pilgrimModelProvider;
            _collection = collection;
            _pilgrimProject = pilgrimProject;
            _childObjects = new ProjectNode[]
                {
                    new WorkItemsNode(_pilgrimProject.Project.QueryHierarchy), 
                    new ReportsNode(),
                    new BuildsNode(pilgrimModelProvider, _collection, _pilgrimProject),
                    new TeamMembersNode(),
                    new SourceControlNode()
                };
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

        public object[] ChildObjects
        {
            get
            {
                return _childObjects;
            }
        }

        private void PilgrimProjectCallback(object state)
        {
            
        }
    }
}
