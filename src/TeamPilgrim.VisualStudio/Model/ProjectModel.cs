using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class ProjectModel : BaseModel, IQueryItemCommandModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public ProjectBuildModel ProjectBuildModel { get; private set; }

        public BaseNode[] ChildObjects { get; private set; }

        public ProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection projectCollection, Project project, ProjectBuildModel projectBuildModel)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            ProjectCollection = projectCollection;
            Project = project;
            ProjectBuildModel = projectBuildModel;
            
            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);
            OpenQueryDefinitionCommand = new RelayCommand<QueryDefinitionNode>(OpenQueryDefinition, CanOpenQueryDefinition);
            
            ChildObjects = new BaseNode[]
                {
                    new WorkItemsNode(Project.QueryHierarchy, this), 
                    new ReportsNode(),
                    new BuildsNode(projectBuildModel),
                    new TeamMembersNode(),
                    new SourceControlNode()
                };
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            ProjectBuildModel.Activate();

            if (ThreadPool.QueueUserWorkItem(PilgrimProjectCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        #region OpenQueryItem

        public RelayCommand<QueryDefinitionNode> OpenQueryDefinitionCommand { get; private set; }

        private void OpenQueryDefinition(QueryDefinitionNode queryDefinitionNode)
        {
            var queryDocument = TeamPilgrimPackage.WorkItemTrackingDocumentService.GetQuery(ProjectCollection, queryDefinitionNode.QueryDefinition.Id.ToString(), this);
            var resultsDocument = TeamPilgrimPackage.WorkItemTrackingDocumentService.CreateLinkResults(queryDocument, this);
            TeamPilgrimPackage.WorkItemTrackingDocumentService.ShowResults(resultsDocument);
        }

        private bool CanOpenQueryDefinition(QueryDefinitionNode queryDefinitionNode)
        {
            return true;
        }

        #endregion

        #region OpenSourceControl

        public RelayCommand OpenSourceControlCommand { get; private set; }

        private void OpenSourceControl()
        {
            VersionControlExplorerExt versionControlExplorerExt = TeamPilgrimPackage.VersionControlExt.Explorer;
            versionControlExplorerExt.Navigate("$/" + Project.Name);
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
