using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels
{
    public class QueryDefinitionModel : QueryItemModel
    {
        private readonly QueryDefinition _queryDefinition;

        public QueryDefinitionModel(QueryDefinition queryDefinition, IQueryItemCommandModel queryItemCommandModel)
            : base(queryItemCommandModel)
        {
            _queryDefinition = queryDefinition;
        }

        public QueryDefinition QueryDefinition
        {
            get { return _queryDefinition; }
        }

        public string Name
        {
            get { return QueryDefinition.Name; }
        }
    }
}