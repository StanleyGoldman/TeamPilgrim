using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public static class EnumerableQueryItemExtensions
    {
        public static QueryItemModel[] GetQueryItemViews(this IEnumerable<QueryItem> queryHierarchy, IQueryItemCommandModel queryItemCommandModel)
        {
            var queryItems = queryHierarchy.Select<QueryItem, QueryItemModel>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        return new QueryFolderModel(queryFolder, queryItemCommandModel);
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new QueryDefinitionModel(queryDefinition, queryItemCommandModel);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
            return queryItems;
        }
    }
}