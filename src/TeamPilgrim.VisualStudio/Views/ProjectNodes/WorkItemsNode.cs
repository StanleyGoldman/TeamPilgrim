using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Views.ProjectNodes
{
    public class WorkItemsNode : ProjectNode
    {
        private readonly QueryHierarchy _queryHierarchy;

        public WorkItemsNode(QueryHierarchy queryHierarchy)
        {
            _queryHierarchy = queryHierarchy;
        }

        public QueryItemView[] QueryItems
        {
            get
            {
                return _queryHierarchy.GetQueryItemViews();
            }
        }
    }
}