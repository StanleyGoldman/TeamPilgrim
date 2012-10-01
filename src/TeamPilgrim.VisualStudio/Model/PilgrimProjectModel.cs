using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using JustAProgrammer.TeamPilgrim.VisualStudio.Views;
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

        public ProjectNode[] ChildObjects { get; private set; }

        public PilgrimProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection collection, Project pilgrimProject, PilgrimProjectBuildModel pilgrimProjectBuildModel)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            PilgrimProject = pilgrimProject;
            Collection = collection;
            PilgrimProjectBuildModel = pilgrimProjectBuildModel;
            OpenSourceControlCommand = new RelayCommand<string>(OpenSourceControl, CanOpenSourceControl);
            ChildObjects = new ProjectNode[]
                {
                    new WorkItemsNode(PilgrimProject.QueryHierarchy), 
                    new ReportsNode(),
                    new BuildsNode(pilgrimProjectBuildModel),
                    new TeamMembersNode(),
                    new SourceControlNode(this)
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

        public RelayCommand<string> OpenSourceControlCommand { get; private set; }

        private void OpenSourceControl(string s)
        {
            VersionControlExplorerExt versionControlExplorerExt = TeamPilgrimPackage.VersionControlExt.Explorer;
            versionControlExplorerExt.Navigate("$/");
        }

        private bool CanOpenSourceControl(string s)
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
