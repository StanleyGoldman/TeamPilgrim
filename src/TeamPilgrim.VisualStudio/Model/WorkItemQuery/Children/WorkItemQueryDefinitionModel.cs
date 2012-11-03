using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public class WorkItemQueryDefinitionModel : WorkItemQueryChildModel
    {
        public QueryDefinition QueryDefinition { get; private set; }

        public WorkItemQueryDefinitionModel(QueryDefinition queryDefinition, IWorkItemQueryCommandModel workItemQueryCommandModel)
            : base(workItemQueryCommandModel)
        {
            QueryDefinition = queryDefinition;
        }

        public override Guid Id
        {
            get { return QueryDefinition.Id; }
        }

        public string Name
        {
            get { return QueryDefinition.Name; }
        }
    }
}