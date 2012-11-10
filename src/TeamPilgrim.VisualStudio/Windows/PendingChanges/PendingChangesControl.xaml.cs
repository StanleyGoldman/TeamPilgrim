using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges
{
    /// <summary>
    /// Interaction logic for TeamPilgrimPendingChangesControl.xaml
    /// </summary>
    public partial class PendingChangesControl : UserControl
    {
        public PendingChangesControl()
        {
            InitializeComponent();
        }

        private void PendingChangeWorkItemCheckbox_CheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedWorkItemModel = checkBox.DataContext as WorkItemModel;
            Debug.Assert(checkedWorkItemModel != null, "checkedWorkItemModel != null");

            var selectedItems = PendingChangesWorkItemsListView.SelectedItems;
            foreach (var workItemModel in selectedItems.Cast<WorkItemModel>().Where(model => model != checkedWorkItemModel))
            {
                workItemModel.IsSelected = checkedWorkItemModel.IsSelected;
            }

            var teamPilgrimModel = (TeamPilgrimServiceModel)DataContext;
            if (teamPilgrimModel.SelectedWorkspaceModel != null)
                teamPilgrimModel.SelectedWorkspaceModel.EvaluateCheckInCommand.Execute(null);
        }

        private void PendingChangesCheckbox_CheckChanged(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            
            Debug.Assert(checkBox != null, "checkBox != null");
            var pendingChangeModel = checkBox.DataContext as PendingChangeModel;

            var selectedItems = PendingChangesListView.SelectedItems;
            foreach (var workItemModel in selectedItems.Cast<PendingChangeModel>().Where(model => model != pendingChangeModel))
            {
                workItemModel.IncludeChange = pendingChangeModel.IncludeChange;
            }

            var teamPilgrimModel = (TeamPilgrimServiceModel)DataContext;
            if (teamPilgrimModel.SelectedWorkspaceModel != null)
                teamPilgrimModel.SelectedWorkspaceModel.EvaluateCheckInCommand.Execute(null);
        }
    }
}
