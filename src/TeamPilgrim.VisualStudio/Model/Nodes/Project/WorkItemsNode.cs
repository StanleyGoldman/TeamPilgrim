using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project
{
    public class WorkItemsNode : BaseNode
    {
        private readonly QueryHierarchy _queryHierarchy;
        private readonly IQueryItemCommandModel _queryItemCommandModel;

        public WorkItemsNode(QueryHierarchy queryHierarchy, IQueryItemCommandModel queryItemCommandModel)
        {
            _queryHierarchy = queryHierarchy;
            _queryItemCommandModel = queryItemCommandModel;
        }

        public QueryItemNode[] QueryItems
        {
            get
            {
                return _queryHierarchy.GetQueryItemViews(_queryItemCommandModel);
            }
        }
    }
}