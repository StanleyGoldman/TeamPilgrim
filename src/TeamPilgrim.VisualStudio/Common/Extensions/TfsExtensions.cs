using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions
{
    public static class TfsTeamProjectCollectionExtensions
    {
        public static VersionControlServer GetVersionControlServer(this TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            return tfsTeamProjectCollection.GetService<VersionControlServer>();
        }
    }
}
