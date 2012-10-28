namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project
{
    public class BuildsNode : BaseNode
    {
        private readonly ProjectBuildModel _projectBuildModel;

        public BuildsNode(ProjectBuildModel projectBuildModel)
        {
            _projectBuildModel = projectBuildModel;
        }

        public ProjectBuildModel ProjectBuildModel
        {
            get { return _projectBuildModel; }
        }
    }
}
