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
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly PilgrimProjectCollection _collection;
        private readonly PilgrimProject _pilgrimProject;
        private readonly ProjectNode[] _childObjects;

        public PilgrimProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, PilgrimProjectCollection collection, PilgrimProject pilgrimProject)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _collection = collection;
            _pilgrimProject = pilgrimProject;
            _childObjects = new ProjectNode[]
                {
                    new WorkItemsNode(_pilgrimProject.Project.QueryHierarchy), 
                    new ReportsNode(),
                    new BuildsNode(pilgrimServiceModelProvider, _collection, _pilgrimProject),
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
