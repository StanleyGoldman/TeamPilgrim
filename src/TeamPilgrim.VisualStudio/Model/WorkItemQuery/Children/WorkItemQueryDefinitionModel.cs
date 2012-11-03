using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public class WorkItemQueryDefinitionModel : WorkItemQueryChildModel
    {
        private readonly QueryDefinition _queryDefinition;

        public WorkItemQueryDefinitionModel(QueryDefinition queryDefinition, IWorkItemQueryCommandModel workItemQueryCommandModel)
            : base(workItemQueryCommandModel)
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