using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectModels
{
    public class WorkItemsModel : BaseModel
    {
        private readonly QueryHierarchy _queryHierarchy;
        private readonly IQueryItemCommandModel _queryItemCommandModel;

        public WorkItemsModel(QueryHierarchy queryHierarchy, IQueryItemCommandModel queryItemCommandModel)
        {
            _queryHierarchy = queryHierarchy;
            _queryItemCommandModel = queryItemCommandModel;
        }

        public QueryItemModel[] QueryItems
        {
            get
            {
                return _queryHierarchy.GetQueryItemViews(_queryItemCommandModel);
            }
        }
    }
}