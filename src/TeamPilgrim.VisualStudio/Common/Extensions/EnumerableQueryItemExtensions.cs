using System;
using System.Collections.Generic;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions
{
    public static class EnumerableQueryItemExtensions
    {
        public static WorkItemQueryChildModel[] GetQueryItemViewModels(this IEnumerable<QueryItem> queryHierarchy, IWorkItemQueryCommandModel workItemQueryCommandModel, ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, Project project, int depth)
        {
            return queryHierarchy.Select<QueryItem, WorkItemQueryChildModel>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        var foldersChildren = queryFolder.GetQueryItemViewModels(workItemQueryCommandModel, teamPilgrimServiceModelProvider, teamPilgrimVsService, project, depth + 1);
                        return new WorkItemQueryFolderModel(workItemQueryCommandModel, project, depth, queryFolder, foldersChildren);
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new WorkItemQueryDefinitionModel(workItemQueryCommandModel, project, depth, queryDefinition);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
        }
    }
}