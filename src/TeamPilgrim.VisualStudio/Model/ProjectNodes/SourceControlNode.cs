namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes
{
    public class SourceControlNode : ProjectNode
    {
        private readonly PilgrimProjectModel _pilgrimProjectModel;

        public SourceControlNode(PilgrimProjectModel pilgrimProjectModel)
        {
            _pilgrimProjectModel = pilgrimProjectModel;
        }

        public PilgrimProjectModel PilgrimProjectModel
        {
            get { return _pilgrimProjectModel; }
        }
    }
}