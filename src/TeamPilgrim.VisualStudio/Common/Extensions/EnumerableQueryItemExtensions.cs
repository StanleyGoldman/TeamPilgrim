using System;
using System.Collections.Generic;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions
{
    public static class EnumerableQueryItemExtensions
    {
        public static WorkItemQueryChildModel[] GetQueryItemViewModels(this IEnumerable<QueryItem> queryHierarchy, IWorkItemQueryCommandModel workItemQueryCommandModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, int depth)
        {
            return queryHierarchy.Select<QueryItem, WorkItemQueryChildModel>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        var foldersChildren = queryFolder.GetQueryItemViewModels(workItemQueryCommandModel, pilgrimServiceModelProvider, teamPilgrimVsService, depth + 1);
                        return new WorkItemQueryFolderModel(pilgrimServiceModelProvider, teamPilgrimVsService, workItemQueryCommandModel, depth, queryFolder, foldersChildren);
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new WorkItemQueryDefinitionModel(pilgrimServiceModelProvider, teamPilgrimVsService, workItemQueryCommandModel, depth, queryDefinition);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
        }
    }
}