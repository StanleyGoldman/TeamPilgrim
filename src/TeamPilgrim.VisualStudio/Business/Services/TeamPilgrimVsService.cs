using System;
using System.Reflection;
using EnvDTE80;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.Build;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class TeamPilgrimVsService : ITeamPilgrimVsService
    {
        protected IVsUIShell VsUiShell { get; private set; }

        public event ActiveProjectContextChanged ActiveProjectContextChangedEvent;

        protected DTE2 Dte2 { get; set; }

        protected TeamFoundationServerExt TeamFoundationServerExt { get; set; }

        protected TeamFoundationHostWrapper TeamFoundationHost { get; set; }

        protected VersionControlExt VersionControlExt { get; set; }

        protected DocumentService WorkItemTrackingDocumentService { get; set; }

        public ProjectContextExt ActiveProjectContext
        {
            get {
                return TeamFoundationServerExt == null ? null : TeamFoundationServerExt.ActiveProjectContext;
            }
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

        public TeamPilgrimVsService()
        {
        }

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
                var fi = typeof(TeamFoundationServerExt).GetField("m_teamFoundationHost", BindingFlags.NonPublic | BindingFlags.Instance);
                var teamFoundationHostObject = fi.GetValue(TeamFoundationServerExt);

                TeamFoundationHost = new TeamFoundationHostWrapper(teamFoundationHostObject);

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

        public void InitializePackage(TeamPilgrimPackage packageInstance, IVsUIShell vsUiShell)
        {
            VsUiShell = vsUiShell;
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

        public void CloseQueryDefinitionFrames(TfsTeamProjectCollection projectCollection, Guid queryDefinitionId)
        {
            var workItemsQueryFrame = GetVsWindowFrameByTypeAndMoniker(VsWindowFrameEditorTypeIds.WorkItemsQueryView, "vstfs:///WorkItemTracking/Query/" + queryDefinitionId.ToString());
            
            if (workItemsQueryFrame != null) 
                workItemsQueryFrame.CloseFrame((int)__FRAMECLOSE.FRAMECLOSE_NoSave);

            var workItemsResultsViewFrame = GetVsWindowFrameByTypeAndMoniker(VsWindowFrameEditorTypeIds.WorkItemsResultView, "vstfs:///WorkItemTracking/Results/" + queryDefinitionId.ToString());
            
            if (workItemsResultsViewFrame != null)
                workItemsResultsViewFrame.CloseFrame((int)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        private IVsWindowFrame GetVsWindowFrameByTypeAndMoniker(Guid editorTypeId, string moniker)
        {
            IEnumWindowFrames enumWindowFrames;
            VsUiShell.GetDocumentWindowEnum(out enumWindowFrames);

            var frames = new IVsWindowFrame[1];
            uint numFrames;

            IVsWindowFrame m_frame;
            while (enumWindowFrames.Next(1, frames, out numFrames) == VSConstants.S_OK && numFrames == 1)
            {
                m_frame = frames[0] as IVsWindowFrame;

                object monikerObject;
                m_frame.GetProperty((int) __VSFPROPID.VSFPROPID_pszMkDocument, out monikerObject);

                Guid editorTypeIdGuid;
                m_frame.GetGuidProperty((int) __VSFPROPID.VSFPROPID_guidEditorType, out editorTypeIdGuid);

                if (monikerObject != null && monikerObject.Equals(moniker) && editorTypeIdGuid.Equals(editorTypeId))
                {
                    return m_frame;
                }
            }

            return null;
        }

        public void OpenBuildDefinition(Uri buildDefinitionUri)
        {
            TeamFoundationBuild.DetailsManager.OpenBuild(buildDefinitionUri);
        }

        public void QueueBuild(string projectName, Uri buildDefinitionUri)
        {
            TeamFoundationBuild.DetailsManager.QueueBuild(projectName, buildDefinitionUri);
        }

        public void NewBuildDefinition(string projectName)
        {
            var buildDefinition = TeamFoundationBuild.BuildServer.CreateBuildDefinition(projectName);
        }

        public void ViewBuilds(string projectName, string buildDefinition, string qualityFilter, DateFilter dateFilter)
        {
            TeamFoundationBuild.BuildExplorer.CompletedView.Show(projectName, buildDefinition, qualityFilter, dateFilter);
        }

        public void TfsConnect()
        {
            TeamFoundationHost.PromptForServerAndProjects(false);
        }
    }
}