using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public class WorkItemQueryFolderModel : WorkItemQueryChildModel
    {
        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public QueryFolder QueryFolder { get; private set; }

        public WorkItemQueryFolderModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, IWorkItemQueryCommandModel workItemQueryCommandModel, QueryFolder queryFolder, IEnumerable<WorkItemQueryChildModel> childQueryItemViewModels)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService, workItemQueryCommandModel)
        {
            QueryFolder = queryFolder;

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