using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Views
{
    public class QueryDefinitionView : QueryItemView
    {
        private readonly QueryDefinition _queryDefinition;

        public QueryDefinitionView(QueryDefinition queryDefinition)
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