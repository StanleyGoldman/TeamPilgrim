using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces
{
    public interface ITeamPilgrimBuildService
    {
        BuildDetailWrapper[] QueryBuildDetails(string teamProject);
        BuildDefinitionWrapper[] QueryBuildDefinitions(string teamProject);
    }
}