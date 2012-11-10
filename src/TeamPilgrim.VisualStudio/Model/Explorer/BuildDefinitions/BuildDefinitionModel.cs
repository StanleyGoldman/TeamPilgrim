using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer.BuildDefinitions
{
    public class BuildDefinitionModel : BaseModel
    {
        public IBuildDefinitionCommandModel BuildDefinitionCommandModel { get; private set; }
        public IBuildDefinition Definition { get; private set; }

        public BuildDefinitionModel(IBuildDefinitionCommandModel buildDefinitionCommandModel, IBuildDefinition definition)
        {
            Definition = definition;
            BuildDefinitionCommandModel = buildDefinitionCommandModel;
        }
    }
}
