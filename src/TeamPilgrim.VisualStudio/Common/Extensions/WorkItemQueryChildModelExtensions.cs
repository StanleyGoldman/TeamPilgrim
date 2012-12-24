using System.Collections.Generic;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public static class WorkItemQueryChildModelExtensions
    {
        public static WorkItemQueryChildModel FindWorkItemQueryChildModelMatchingPath(this IEnumerable<WorkItemQueryChildModel> workItemQueryChildModel, string path)
        {
            foreach (var itemQueryChildModel in workItemQueryChildModel)
            {
                var workItemQueryDefinitionModel = itemQueryChildModel as WorkItemQueryDefinitionModel;
                if (workItemQueryDefinitionModel != null)
                {
                    if (workItemQueryDefinitionModel.QueryDefinition.Path == path)
                    {
                        return workItemQueryDefinitionModel;
                    }
                }

                var workItemQueryFolderModel = itemQueryChildModel as WorkItemQueryFolderModel;
                if (workItemQueryFolderModel != null)
                {
                    if (workItemQueryFolderModel.QueryFolder.Path == path)
                    {
                        return workItemQueryFolderModel;
                    }

                    var childResult = workItemQueryFolderModel.QueryItems.FindWorkItemQueryChildModelMatchingPath(path);
                    if (childResult != null)
                        return childResult;
                }
            }

            return null;
        }
    }
}
