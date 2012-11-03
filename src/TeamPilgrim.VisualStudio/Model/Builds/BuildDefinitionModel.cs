using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Builds
{
    public class BuildDefinitionModel : BaseModel
    {
        public IBuildDefinitionCommandModel BuildDefinitionCommandModel { get; private set; }
        public IBuildDefinition Definition { get; private set; }

        public BuildDefinitionModel(IBuildDefinitionCommandModel buildDefinitionCommandModel, IBuildDefinition definition)
        {
            BuildDefinitionCommandModel = buildDefinitionCommandModel;
            Definition = definition;
        }
    }
}
