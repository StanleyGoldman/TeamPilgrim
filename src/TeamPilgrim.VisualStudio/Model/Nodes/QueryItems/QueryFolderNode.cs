using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems
{
    public class QueryFolderNode : QueryItemNode
    {
        private readonly QueryFolder _folder;

        public QueryFolderNode(QueryFolder folder, IQueryItemCommandModel queryItemCommandModel)
            : base(queryItemCommandModel)
        {
            _folder = folder;
        }

        private QueryFolder Folder
        {
            get { return _folder; }
        }

        public string Name
        {
            get { return Folder.Name; }
        }

        public QueryItemNode[] QueryItems
        {
            get
            {
                return _folder.GetQueryItemViews(queryItemCommandModel);
            }
        }
    }
}