﻿using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class SelectWorkItemQueryModel : BaseModel
    {
        public ProjectCollectionModel ActiveProjectCollectionModel { get; private set; }

        private BaseModel _selectedItem;
        public BaseModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            private set
            {
                if (_selectedItem == value) return;

                _selectedItem = value;

                SendPropertyChanged("SelectedItem");

                SelectedWorkItemQueryDefinitionModel = _selectedItem as WorkItemQueryDefinitionModel;
            }
        }

        private WorkItemQueryDefinitionModel _selectedWorkWorkItemQueryDefinitionModelQueryDefinitionModel;
        public WorkItemQueryDefinitionModel SelectedWorkItemQueryDefinitionModel
        {
            get
            {
                return _selectedWorkWorkItemQueryDefinitionModelQueryDefinitionModel;
            }
            private set
            {
                if (_selectedWorkWorkItemQueryDefinitionModelQueryDefinitionModel == value) return;

                _selectedWorkWorkItemQueryDefinitionModelQueryDefinitionModel = value;

                SendPropertyChanged("SelectedWorkItemQueryDefinitionModel");
            }
        }

        public SelectWorkItemQueryModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionModel activeProjectCollectionModel)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ActiveProjectCollectionModel = activeProjectCollectionModel;
        }
    }
}
