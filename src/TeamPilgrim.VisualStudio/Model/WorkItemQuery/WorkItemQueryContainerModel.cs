using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public class WorkItemQueryContainerModel : BaseModel, IWorkItemQueryCommandModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public ObservableCollection<WorkItemQueryChildModel> QueryItems { get; private set; }

        public WorkItemQueryContainerModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection projectCollection, Project project)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            ProjectCollection = projectCollection;
            Project = project;

            OpenQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(OpenQueryDefinition, CanOpenQueryDefinition);
            EditQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(EditQueryDefinition, CanEditQueryDefinition);
            DeleteQueryDefinitionCommand = new RelayCommand<WorkItemQueryDefinitionModel>(DeleteQueryDefinition, CanDeleteQueryDefinition);

            var queryHierarchy = Project.QueryHierarchy;
            var queryItemModels = queryHierarchy.GetQueryItemViewModels(this);

            QueryItems = new ObservableCollection<WorkItemQueryChildModel>(queryItemModels);
        }

        #region OpenQueryDefinition

        public RelayCommand<WorkItemQueryDefinitionModel> OpenQueryDefinitionCommand { get; private set; }

        private void OpenQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.OpenQueryDefinition(ProjectCollection, workItemQueryDefinitionModel.QueryDefinition.Id);
        }

        private bool CanOpenQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region EditQueryDefinition

        public RelayCommand<WorkItemQueryDefinitionModel> EditQueryDefinitionCommand { get; private set; }

        private void EditQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            TeamPilgrimPackage.TeamPilgrimVsService.EditQueryDefinition(ProjectCollection, workItemQueryDefinitionModel.QueryDefinition.Id);
        }

        private bool CanEditQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion

        #region DeleteQueryDefinition

        public RelayCommand<WorkItemQueryDefinitionModel> DeleteQueryDefinitionCommand { get; private set; }

        private void DeleteQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            bool result;

            var queryId = workItemQueryDefinitionModel.QueryDefinition.Id;

            if (_pilgrimServiceModelProvider.TryDeleteQueryDefinition(out result, ProjectCollection, Project, queryId))
            {
                if (result)
                {
                    TeamPilgrimPackage.TeamPilgrimVsService.CloseQueryDefinitionFrames(ProjectCollection, queryId);
                    workItemQueryDefinitionModel.ParentQueryFolderModel.QueryItems.Remove(workItemQueryDefinitionModel);
                }
            }
        }

        private bool CanDeleteQueryDefinition(WorkItemQueryDefinitionModel workItemQueryDefinitionModel)
        {
            return true;
        }

        #endregion
    }
}