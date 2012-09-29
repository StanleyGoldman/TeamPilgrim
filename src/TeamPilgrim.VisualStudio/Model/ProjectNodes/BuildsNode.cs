namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes
{
    public class BuildsNode : ProjectNode
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
