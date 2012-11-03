using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TeamFoundation.Build;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio
{
    public class VsTeamFoundationBuildWrapper : IVsTeamFoundationBuild
    {
        private readonly IVsTeamFoundationBuild _vsTeamFoundationBuild;
        private readonly Lazy<BuildExplorerWrapper> _buildExplorerWrapper;

        public VsTeamFoundationBuildWrapper(IVsTeamFoundationBuild vsTeamFoundationBuild)
        {
            _vsTeamFoundationBuild = vsTeamFoundationBuild;
            _buildExplorerWrapper = new Lazy<BuildExplorerWrapper>(() => new BuildExplorerWrapper(_vsTeamFoundationBuild.BuildExplorer));
        }

        public IBuildExplorer BuildExplorer
        {
            get { return _buildExplorerWrapper.Value; }
        }

        public BuildExplorerWrapper BuildExplorerWrapper
        {
            get { return _buildExplorerWrapper.Value; }
        }

        public IBuildDefinitionManager DefinitionManager
        {
            get { return _vsTeamFoundationBuild.DefinitionManager; }
        }

        public IBuildDetailManager DetailsManager
        {
            get { return _vsTeamFoundationBuild.DetailsManager; }
        }

        public IBuildControllerManager ControllerManager
        {
            get { return _vsTeamFoundationBuild.ControllerManager; }
        }

        public IBuildQualityManager QualityManager
        {
            get { return _vsTeamFoundationBuild.QualityManager; }
        }

        public IBuildServer BuildServer
        {
            get { return _vsTeamFoundationBuild.BuildServer; }
        }
    }
}