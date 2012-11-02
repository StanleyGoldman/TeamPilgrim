using System.Collections.Generic;
using System.Collections.ObjectModel;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectModels
{
    public class WorkItemsModel : BaseModel
    {
        public ObservableCollection<QueryItemModel> QueryItems { get; private set; }

        public WorkItemsModel(IEnumerable<QueryItemModel> queryItemModels)
        {
            QueryItems = new ObservableCollection<QueryItemModel>(queryItemModels);
        }
    }
}