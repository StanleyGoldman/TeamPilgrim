using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TfsTeamProjectCollection Collection { get; private set; }

        public Project PilgrimProject { get; private set; }

        public PilgrimProjectBuildModel PilgrimProjectBuildModel { get; private set; }

        public BaseNode[] ChildObjects { get; private set; }

        public PilgrimProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection collection, Project pilgrimProject, PilgrimProjectBuildModel pilgrimProjectBuildModel)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            PilgrimProject = pilgrimProject;
            Collection = collection;
            PilgrimProjectBuildModel = pilgrimProjectBuildModel;
            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);
            ChildObjects = new BaseNode[]
                {
                    new WorkItemsNode(PilgrimProject.QueryHierarchy), 
                    new ReportsNode(),
                    new BuildsNode(pilgrimProjectBuildModel),
                    new TeamMembersNode(),
                    new SourceControlNode()
                };
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            PilgrimProjectBuildModel.Activate();

            if (ThreadPool.QueueUserWorkItem(PilgrimProjectCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        #region OpenSourceControl

        public RelayCommand OpenSourceControlCommand { get; private set; }

        private void OpenSourceControl()
        {
            VersionControlExplorerExt versionControlExplorerExt = TeamPilgrimPackage.VersionControlExt.Explorer;
            versionControlExplorerExt.Navigate("$/" + PilgrimProject.Name);
        }

        private bool CanOpenSourceControl()
        {
            return true;
        }

        #endregion


        private void PilgrimProjectCallback(object state)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    State = ModelStateEnum.Active;
                }));
        }


    }
}
