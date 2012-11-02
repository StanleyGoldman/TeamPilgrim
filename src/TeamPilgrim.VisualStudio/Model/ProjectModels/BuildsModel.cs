namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectModels
{
    public class BuildsModel : BaseModel
    {
        private readonly ProjectBuildModel _projectBuildModel;

        public BuildsModel(ProjectBuildModel projectBuildModel)
        {
            _projectBuildModel = projectBuildModel;
        }

        public ProjectBuildModel ProjectBuildModel
        {
            get { return _projectBuildModel; }
        }
    }
}
