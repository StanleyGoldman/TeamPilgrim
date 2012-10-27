using System;
using System.Collections.Generic;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public static class EnumerableQueryItemExtensions
    {
        public static QueryItemNode[] GetQueryItemViews(this IEnumerable<QueryItem> queryHierarchy)
        {
            var queryItems = queryHierarchy.Select<QueryItem, QueryItemNode>(item =>
                {
                    var queryFolder = item as QueryFolder;
                    if (queryFolder != null)
                    {
                        return new QueryFolderNode(queryFolder);
                    }

                    var queryDefinition = item as QueryDefinition;
                    if (queryDefinition != null)
                    {
                        return new QueryDefinitionNode(queryDefinition);
                    }

                    throw new ArgumentException(item.GetType().ToString());
                }).ToArray();
            return queryItems;
        }
    }
}