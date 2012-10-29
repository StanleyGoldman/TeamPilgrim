using System;
using EnvDTE80;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.Build;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class TeamPilgrimVsService : ITeamPilgrimVsService
    {
        public event ActiveProjectContextChanged ActiveProjectContextChangedEvent;

        protected DTE2 Dte2 { get; set; }

        protected TeamFoundationServerExt TeamFoundationServerExt { get; set; }

        protected VersionControlExt VersionControlExt { get; set; }

        protected DocumentService WorkItemTrackingDocumentService { get; set; }

        public ProjectContextExt ActiveProjectContext
        {
            get { return TeamFoundationServerExt.ActiveProjectContext; }
        }

        private TeamPilgrimPackage _packageInstance;

        private IVsTeamFoundationBuild _teamFoundationBuild;
        private IVsTeamFoundationBuild TeamFoundationBuild
        {
            get
            {
                return _teamFoundationBuild ?? (_teamFoundationBuild = _packageInstance.GetPackageService<IVsTeamFoundationBuild>());
            }
        }

        private IWorkItemControlHost _workItemControlHost;
        private IWorkItemControlHost WorkItemControlHost
        {
            get
            {
                return _workItemControlHost ?? (_workItemControlHost = _packageInstance.GetPackageService<IWorkItemControlHost>());
            }
        }

        public void InitializeGlobals(DTE2 dte2)
        {
            Dte2 = dte2;
            VersionControlExt = dte2.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            TeamFoundationServerExt = dte2.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
            WorkItemTrackingDocumentService = dte2.GetObject("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.DocumentService") as DocumentService;

            if (TeamFoundationServerExt != null)
            {
                TeamFoundationServerExt.ProjectContextChanged += TeamFoundationServerExtOnProjectContextChanged;

                if (TeamFoundationServerExt.ActiveProjectContext != null)
                {
                    TeamFoundationServerExtOnProjectContextChanged(null, EventArgs.Empty);
                }
            }
        }


        private void TeamFoundationServerExtOnProjectContextChanged(object sender, EventArgs e)
        {
            ActiveProjectContextChangedEvent(TeamFoundationServerExt.ActiveProjectContext);
        }

        public void InitializePackage(TeamPilgrimPackage packageInstance)
        {
            _packageInstance = packageInstance;
        }

        public void OpenSourceControl(string projectName)
        {
            VersionControlExplorerExt versionControlExplorerExt = VersionControlExt.Explorer;
            versionControlExplorerExt.Navigate("$/" + projectName);
        }

        public void OpenQueryDefinition(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId)
        {
            var queryDocument = WorkItemTrackingDocumentService.GetQuery(projectCollection, queryDefinitionId.ToString(), this);

            var resultsDocument = WorkItemTrackingDocumentService.GetLinkResults(queryDocument, this) ??
                                            WorkItemTrackingDocumentService.CreateLinkResults(queryDocument, this);

            WorkItemTrackingDocumentService.ShowResults(resultsDocument);
        }

        public void EditQueryDefinition(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId)
        {
            var queryDocument = WorkItemTrackingDocumentService.GetQuery(projectCollection, queryDefinitionId.ToString(), this);

            WorkItemTrackingDocumentService.ShowQuery(queryDocument);
        }

        public void OpenBuildDefinition(Uri uri)
        {
            TeamFoundationBuild.DetailsManager.OpenBuild(uri);
        }

        public void ViewBuilds(string name, string s, string empty, DateFilter today)
        {
            TeamFoundationBuild.BuildExplorer.CompletedView.Show(name, s, empty, today);
        }
    }
}