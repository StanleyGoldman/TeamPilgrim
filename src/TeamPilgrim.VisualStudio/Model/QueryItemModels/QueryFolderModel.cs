using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels
{
    public class QueryFolderModel : QueryItemModel
    {
        private readonly QueryFolder _folder;

        public QueryFolderModel(QueryFolder folder, IQueryItemCommandModel queryItemCommandModel)
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

        public QueryItemModel[] QueryItems
        {
            get
            {
                return _folder.GetQueryItemViews(queryItemCommandModel);
            }
        }
    }
}