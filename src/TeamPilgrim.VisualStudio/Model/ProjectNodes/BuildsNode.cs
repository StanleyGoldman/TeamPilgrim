namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes
{
    public class BuildsNode : ProjectNode
    {
        private readonly PilgrimProjectBuildModel _pilgrimServiceModelProvider;

        public BuildsNode(PilgrimProjectBuildModel pilgrimServiceModelProvider)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
        }

        public PilgrimProjectBuildModel PilgrimServiceModelProvider
        {
            get { return _pilgrimServiceModelProvider; }
        }
    }
}
