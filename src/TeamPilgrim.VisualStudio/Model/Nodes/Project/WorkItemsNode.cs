using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project
{
    public class WorkItemsNode : BaseNode
    {
        private readonly QueryHierarchy _queryHierarchy;

        public WorkItemsNode(QueryHierarchy queryHierarchy)
        {
            _queryHierarchy = queryHierarchy;
        }

        public QueryItemNode[] QueryItems
        {
            get
            {
                return _queryHierarchy.GetQueryItemViews();
            }
        }
    }
}