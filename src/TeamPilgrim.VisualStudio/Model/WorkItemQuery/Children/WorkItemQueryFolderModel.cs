using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public class WorkItemQueryFolderModel : WorkItemQueryChildModel
    {
        private readonly QueryFolder _folder;

        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public WorkItemQueryFolderModel(QueryFolder folder, IWorkItemQueryCommandModel workItemQueryCommandModel, IEnumerable<WorkItemQueryChildModel> childQueryItemViewModels)
            : base(workItemQueryCommandModel)
        {
            _folder = folder;
            QueryItems = new ObservableCollection<WorkItemQueryChildModel>(childQueryItemViewModels);
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