using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities
{
    public class BuildDetailWrapper
    {
        private readonly IBuildDetail _buildDetail;

        public BuildDetailWrapper(IBuildDetail buildDetail)
        {
            _buildDetail = buildDetail;
        }
    }
}
