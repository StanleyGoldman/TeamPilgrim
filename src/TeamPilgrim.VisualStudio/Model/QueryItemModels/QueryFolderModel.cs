using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels
{
    public class QueryFolderModel : QueryItemModel
    {
        private readonly QueryFolder _folder;

        public ObservableCollection<QueryItemModel> QueryItems { get; private set; }

        public QueryFolderModel(QueryFolder folder, IQueryItemCommandModel queryItemCommandModel, IEnumerable<QueryItemModel> childQueryItemViewModels)
            : base(queryItemCommandModel)
        {
            _folder = folder;
            QueryItems = new ObservableCollection<QueryItemModel>(childQueryItemViewModels);
        }

        private QueryFolder Folder
        {
            get { return _folder; }
        }

        public string Name
        {
            get { return Folder.Name; }
        }
    }
}