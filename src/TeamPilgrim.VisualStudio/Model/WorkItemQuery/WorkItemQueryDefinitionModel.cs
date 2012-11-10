using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public class WorkItemQueryDefinitionModel : WorkItemQueryChildModel
    {
        public QueryDefinition QueryDefinition { get; private set; }

        public Project Project { get; private set; }

        public WorkItemQueryDefinitionModel(IWorkItemQueryCommandModel workItemQueryCommandModel, Project project, int depth, QueryDefinition queryDefinition)
            : base(workItemQueryCommandModel, depth)
        {
            QueryDefinition = queryDefinition;
            Project = project;
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