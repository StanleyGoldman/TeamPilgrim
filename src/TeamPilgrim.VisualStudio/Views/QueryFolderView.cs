using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Views
{
    public class QueryFolderView : QueryItemView
    {
        private readonly QueryFolder _folder;

        public QueryFolderView(QueryFolder folder)
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

        public QueryItemView[] QueryItems
        {
            get
            {
                return _folder.GetQueryItemViews();
            }
        }
    }
}