using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Views;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly PilgrimProjectCollection _collection;
        private readonly PilgrimProject _pilgrimProject;
        private readonly ProjectNode[] _childObjects;
        private readonly PilgrimProjectBuildModel _pilgrimProjectBuildModel;

        public PilgrimProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, PilgrimProjectCollection collection, PilgrimProject pilgrimProject, PilgrimProjectBuildModel pilgrimProjectBuildModel)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _pilgrimProjectBuildModel = pilgrimProjectBuildModel;
            _collection = collection;
            _pilgrimProject = pilgrimProject;
            _childObjects = new ProjectNode[]
                {
                    new WorkItemsNode(_pilgrimProject.Project.QueryHierarchy), 
                    new ReportsNode(),
                    new BuildsNode(pilgrimProjectBuildModel),
                    new TeamMembersNode(),
                    new SourceControlNode()
                };
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            _pilgrimProjectBuildModel.Activate();

            if (ThreadPool.QueueUserWorkItem(PilgrimProjectCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        public string Name
        {
            get { return _pilgrimProject.Project.Name; }
        }

        public ProjectNode[] ChildObjects
        {
            get
            {
                return _childObjects;
            }
        }

        public PilgrimProjectBuildModel PilgrimProjectBuildModel
        {
            get { return _pilgrimProjectBuildModel; }
        }

        private void PilgrimProjectCallback(object state)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    State = ModelStateEnum.Active;
                }));
        }
    }
}
