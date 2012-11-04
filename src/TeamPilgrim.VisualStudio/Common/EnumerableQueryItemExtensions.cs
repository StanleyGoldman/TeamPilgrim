using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public static class EnumerableQueryItemExtensions
    {
        public static WorkItemQueryChildModel[] GetQueryItemViewModels(this IEnumerable<QueryItem> queryHierarchy, IWorkItemQueryCommandModel workItemQueryCommandModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
        {
            return queryHierarchy.Select<QueryItem, WorkItemQueryChildModel>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        return new WorkItemQueryFolderModel(pilgrimServiceModelProvider, teamPilgrimVsService, workItemQueryCommandModel, queryFolder, queryFolder.GetQueryItemViewModels(workItemQueryCommandModel, pilgrimServiceModelProvider, teamPilgrimVsService));
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new WorkItemQueryDefinitionModel(pilgrimServiceModelProvider, teamPilgrimVsService, workItemQueryCommandModel, queryDefinition);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
        }
    }
}