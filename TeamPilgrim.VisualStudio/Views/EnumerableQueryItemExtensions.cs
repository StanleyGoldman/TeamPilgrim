using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Views
{
    public static class EnumerableQueryItemExtensions
    {
        public static QueryItemView[] GetQueryItemViews(this IEnumerable<QueryItem> queryHierarchy)
        {
            var queryItems = queryHierarchy.Select<QueryItem, QueryItemView>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        return new QueryFolderView(queryFolder);
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new QueryDefinitionView(queryDefinition);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
            return queryItems;
        }
    }
}