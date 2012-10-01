using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Views;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly TfsTeamProjectCollection _collection;
        private readonly Project _pilgrimProject;
        private readonly ProjectNode[] _childObjects;
        private readonly PilgrimProjectBuildModel _pilgrimProjectBuildModel;

        public PilgrimProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection collection, Project pilgrimProject, PilgrimProjectBuildModel pilgrimProjectBuildModel)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _pilgrimProjectBuildModel = pilgrimProjectBuildModel;
            _collection = collection;
            _pilgrimProject = pilgrimProject;
            _childObjects = new ProjectNode[]
                {
                    new WorkItemsNode(_pilgrimProject.QueryHierarchy), 
                    new ReportsNode(),
                    new BuildsNode(pilgrimProjectBuildModel),
                    new TeamMembersNode(),
                    new SourceControlNode(this)
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
            get { return _pilgrimProject.Name; }
        }

        public Uri Path
        {
            get
            {
                var tfsTeamProjectCollection = new TfsTeamProjectCollection(_collection.Uri);
                var versionControlServer = tfsTeamProjectCollection.GetService<VersionControlServer>();

                return _pilgrimProject.Store.TeamProjectCollection.Uri;
            }
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
