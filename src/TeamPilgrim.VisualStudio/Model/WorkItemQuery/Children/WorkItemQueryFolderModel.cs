using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public class WorkItemQueryFolderModel : WorkItemQueryChildModel
    {
        private readonly QueryFolder _queryFolder;

        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public WorkItemQueryFolderModel(QueryFolder queryFolder, IWorkItemQueryCommandModel workItemQueryCommandModel, IEnumerable<WorkItemQueryChildModel> childQueryItemViewModels)
            : base(workItemQueryCommandModel)
        {
            _queryFolder = queryFolder;

            var childQueryItemViewModelsArray = childQueryItemViewModels.ToArray();

            foreach (var workItemQueryChildModel in childQueryItemViewModelsArray)
            {
                workItemQueryChildModel.ParentQueryFolder = this;
            }

            QueryItems = new ObservableCollection<WorkItemQueryChildModel>(childQueryItemViewModelsArray);
        }

        public QueryFolder QueryFolder
        {
            get { return _queryFolder; }
        }

        public string Name
        {
            get { return QueryFolder.Name; }
        }
    }
}