using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Views
{
    public class QueryHierachyView
    {
        private readonly QueryHierarchy _queryHierarchy;

        public QueryHierachyView(QueryHierarchy queryHierarchy)
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