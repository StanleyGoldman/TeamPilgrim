using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class SelectWorkItemQueryModel : BaseModel
    {
        public ProjectCollectionServiceModel ActiveProjectCollectionModel { get; private set; }

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

        public SelectWorkItemQueryModel(ProjectCollectionServiceModel activeProjectCollectionServiceModel)
        {
            ActiveProjectCollectionModel = activeProjectCollectionServiceModel;
        }
    }
}
