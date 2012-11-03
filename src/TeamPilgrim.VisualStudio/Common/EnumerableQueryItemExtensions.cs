using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public static class EnumerableQueryItemExtensions
    {
        public static WorkItemQueryChildModel[] GetQueryItemViewModels(this IEnumerable<QueryItem> queryHierarchy, IWorkItemQueryCommandModel workItemQueryCommandModel)
        {
            return queryHierarchy.Select<QueryItem, WorkItemQueryChildModel>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        return new WorkItemQueryFolderModel(queryFolder, workItemQueryCommandModel, queryFolder.GetQueryItemViewModels(workItemQueryCommandModel));
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new WorkItemQueryDefinitionModel(queryDefinition, workItemQueryCommandModel);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
        }
    }
}