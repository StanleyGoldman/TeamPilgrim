namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project
{
    public class BuildsNode : BaseNode
    {
        private readonly PilgrimProjectBuildModel _pilgrimProjectBuildModel;

        public BuildsNode(PilgrimProjectBuildModel pilgrimProjectBuildModel)
        {
            _pilgrimProjectBuildModel = pilgrimProjectBuildModel;
        }

        public PilgrimProjectBuildModel PilgrimProjectBuildModel
        {
            get { return _pilgrimProjectBuildModel; }
        }
    }
}
