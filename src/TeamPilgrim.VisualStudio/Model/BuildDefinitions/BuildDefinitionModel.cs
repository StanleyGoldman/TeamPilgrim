using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.BuildDefinitions
{
    public class BuildDefinitionModel : BaseModel
    {
        public IBuildDefinitionCommandModel BuildDefinitionCommandModel { get; private set; }
        public IBuildDefinition Definition { get; private set; }

        public BuildDefinitionModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, IBuildDefinitionCommandModel buildDefinitionCommandModel, IBuildDefinition definition)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            Definition = definition;
            BuildDefinitionCommandModel = buildDefinitionCommandModel;
        }
    }
}
