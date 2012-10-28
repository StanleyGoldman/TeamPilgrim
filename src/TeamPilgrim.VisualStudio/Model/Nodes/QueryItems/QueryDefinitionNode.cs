using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems
{
    public class QueryDefinitionNode : QueryItemNode
    {
        private readonly QueryDefinition _queryDefinition;

        public QueryDefinitionNode(QueryDefinition queryDefinition, IQueryItemCommandModel queryItemCommandModel)
            : base(queryItemCommandModel)
        {
            _queryDefinition = queryDefinition;
        }

        private QueryDefinition QueryDefinition
        {
            get { return _queryDefinition; }
        }

        public string Name
        {
            get { return QueryDefinition.Name; }
        }
    }
}