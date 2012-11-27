using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
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

            NameScope.SetNameScope(PendingChangesContextMenu, NameScope.GetNameScope(this));

            Messenger.Default.Register<ShowPendingChangesTabItemMessage>(this, message =>
            {
                switch (message.ShowPendingChangesTabItem)
                {
                    case ShowPendingChangesTabItemEnum.PolicyWarnings:
                        PolicyWarningsRadioButton.IsChecked = true;
                        break;

                    case ShowPendingChangesTabItemEnum.CheckinNotes:
                        CheckInNotesRadioButton.IsChecked = true;
                        break;

                    case ShowPendingChangesTabItemEnum.WorkItems:
                        WorkItemsRadioButton.IsChecked = true;
                        break;

                    case ShowPendingChangesTabItemEnum.SourceFiles:
                        SourceFilesRadioButton.IsChecked = true;
                        break;
                }
            });
        }

        private void PendingChangeWorkItemCheckboxClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var teamPilgrimModel = (TeamPilgrimServiceModel)DataContext;

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

            teamPilgrimModel.SelectedWorkspaceModel.SelectWorkItemsCommand.Execute(new SelectWorkItemsCommandArgument()
            {
                Collection = collection,
                Value = checkedWorkItemModel.IsSelected
            });
        }

        private void PendingChangesCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var teamPilgrimModel = (TeamPilgrimServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedPendingChangeModel = checkBox.DataContext as PendingChangeModel;
            Debug.Assert(checkedPendingChangeModel != null, "pendingChangeModel != null");

            var selectedPendingChangeModels = PendingChangesListView.SelectedItems.Cast<PendingChangeModel>().ToArray();

            if (selectedPendingChangeModels.Length <= 1)
                return;
            
            if (!selectedPendingChangeModels.Contains(checkedPendingChangeModel))
                return;

            e.Handled = true;

            var collection = selectedPendingChangeModels;

            teamPilgrimModel.SelectedWorkspaceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument()
                {
                    Collection = collection,
                    Value = checkedPendingChangeModel.IncludeChange
                });
        }
    }
}
