using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ShelveChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs
{
    /// <summary>
    /// Interaction logic for ShelveChangesDialog.xaml
    /// </summary>
    public partial class ShelveChangesDialog : Window
    {
        public ShelveChangesDialog()
        {
            InitializeComponent();
        }

        private ShelvesetServiceModel ShelvesetServiceModelDataContext
        {
            get { return (ShelvesetServiceModel) DataContext; }
        }

        private void PendingChangesCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var shelvesetServiceModel = (ShelvesetServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedPendingChangeModel = checkBox.DataContext as PendingChangeModel;
            Debug.Assert(checkedPendingChangeModel != null, "pendingChangeModel != null");

            var selectedPendingChangeModels = PendingChangesListView.SelectedItems.Cast<PendingChangeModel>().ToArray();

            if (selectedPendingChangeModels.Length <= 1)
                return;

            if (!selectedPendingChangeModels.Contains(checkedPendingChangeModel))
                return;


            var collection = selectedPendingChangeModels;

            shelvesetServiceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument()
                {
                    Collection = collection,
                    Value = checkedPendingChangeModel.IncludeChange
                });
        }

        private void PendingChangeWorkItemCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var shelvesetServiceModel = (ShelvesetServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedWorkItemModel = checkBox.DataContext as WorkItemModel;
            Debug.Assert(checkedWorkItemModel != null, "checkedWorkItemModel != null");

            var selectedPendingChangeModels = WorkItemsListView.SelectedItems.Cast<WorkItemModel>().ToArray();

            if (selectedPendingChangeModels.Length <= 1)
                return;

            if (!selectedPendingChangeModels.Contains(checkedWorkItemModel))
                return;

            e.Handled = true;

            var collection = selectedPendingChangeModels;

            shelvesetServiceModel.SelectWorkItemsCommand.Execute(new SelectWorkItemsCommandArgument()
                {
                    Collection = collection,
                    Value = checkedWorkItemModel.IsSelected
                });
        }

        private void OnShelveClick(object sender, RoutedEventArgs e)
        {
            ShelvesetServiceModelDataContext.ShelveCommand.Execute(null);
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
