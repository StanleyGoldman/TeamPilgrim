using System;
using System.Reflection;
using Microsoft.VisualStudio.TeamFoundation.Build;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio
{
    public class BuildExplorerWrapper : IBuildExplorer
    {
        private readonly IBuildExplorer _buildExplorer;
        private readonly Type _buildExplorerType;
        private readonly Lazy<MethodInfo> _addBuildDefinitionMethod;
        private readonly Lazy<MethodInfo> _openControllerAgentManagerMethod;
        private readonly Lazy<MethodInfo> _openQualityManagerMethod;

        public BuildExplorerWrapper(IBuildExplorer buildExplorer)
        {
            _buildExplorer = buildExplorer;
            _buildExplorerType = _buildExplorer.GetType();
            _addBuildDefinitionMethod = new Lazy<MethodInfo>(() => _buildExplorerType.GetMethod("AddBuildDefinition", BindingFlags.Static | BindingFlags.Public));
            _openControllerAgentManagerMethod = new Lazy<MethodInfo>(() => _buildExplorerType.GetMethod("OpenControllerAgentManager", BindingFlags.Static | BindingFlags.Public));
            _openQualityManagerMethod = new Lazy<MethodInfo>(() => _buildExplorerType.GetMethod("OpenQualityManager", BindingFlags.Static | BindingFlags.Public));
        }

        public ICompletedView CompletedView
        {
            get { return _buildExplorer.CompletedView; }
        }

        public bool IsShowing
        {
            get { return _buildExplorer.IsShowing; }
        }

        public IQueuedView QueuedView
        {
            get { return _buildExplorer.QueuedView; }
        }

        public string TeamProjectName
        {
            get { return _buildExplorer.TeamProjectName; }
        }

        public void AddBuildDefinition(string projectName)
        {
            _addBuildDefinitionMethod.Value.Invoke(null, new object[] { projectName });
        }

        public void OpenControllerAgentManager(string projectName)
        {
            _openControllerAgentManagerMethod.Value.Invoke(null, new object[] {projectName});
        }

        public void OpenQualityManager(string projectName)
        {
            _openQualityManagerMethod.Value.Invoke(null, new object[] { projectName });
        }
    }
}