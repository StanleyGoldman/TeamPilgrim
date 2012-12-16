using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public class WorkItemQueryFolderModel : WorkItemQueryChildModel
    {
        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public QueryFolder QueryFolder { get; private set; }

        public QueryFolderTypeEnum? QueryFolderType { get; private set; }

        public Project Project { get; private set; }

        public WorkItemQueryFolderModel(IWorkItemQueryCommandModel workItemQueryCommandModel, Project project, int depth, QueryFolder queryFolder, IEnumerable<WorkItemQueryChildModel> childQueryItemViewModels, QueryFolderTypeEnum? queryFolderType)
            : base(workItemQueryCommandModel, depth)
        {
            Project = project;
            QueryFolder = queryFolder;
            QueryFolderType = queryFolderType;

            var childQueryItemViewModelsArray = childQueryItemViewModels.ToArray();

            foreach (var workItemQueryChildModel in childQueryItemViewModelsArray)
            {
                workItemQueryChildModel.ParentQueryFolder = this;
            }

            QueryItems = new ObservableCollection<WorkItemQueryChildModel>(childQueryItemViewModelsArray);
        }

        public override Guid Id
        {
            get { return QueryFolder.Id; }
        }


        public string Name
        {
            get { return QueryFolder.Name; }
        }
    }
}