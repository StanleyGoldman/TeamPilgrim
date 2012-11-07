using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
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

                SelectedWorkItemQueryDefinition = _selectedItem as WorkItemQueryDefinitionModel;
            }
        }

        private WorkItemQueryDefinitionModel _selectedWorkWorkItemQueryDefinitionQueryDefinition;
        public WorkItemQueryDefinitionModel SelectedWorkItemQueryDefinition
        {
            get
            {
                return _selectedWorkWorkItemQueryDefinitionQueryDefinition;
            }
            private set
            {
                if (_selectedWorkWorkItemQueryDefinitionQueryDefinition == value) return;

                _selectedWorkWorkItemQueryDefinitionQueryDefinition = value;

                SendPropertyChanged("SelectedWorkItemQueryDefinition");
            }
        }

        public SelectWorkItemQueryModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, ProjectCollectionModel activeProjectCollectionModel)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ActiveProjectCollectionModel = activeProjectCollectionModel;
        }
    }
}
