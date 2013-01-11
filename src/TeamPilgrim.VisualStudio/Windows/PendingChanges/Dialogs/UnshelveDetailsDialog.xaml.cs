using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages.Dismiss;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs
{
    /// <summary>
    /// Interaction logic for UnshelveDetailsDialog.xaml
    /// </summary>
    public partial class UnshelveDetailsDialog : Window
    {
        public UnshelveDetailsDialog()
        {
            InitializeComponent();
        }

        public new object DataContext
        {
            get { return base.DataContext; }
            set
            {
                if (base.DataContext == value)
                    return;

                if (base.DataContext != null)
                {
                    Messenger.Default.Unregister<DismissMessage>(this, DataContext);
                }

                base.DataContext = value;

                var unshelveDetailsServiceModel = (UnshelveDetailsServiceModel)value;
                if (unshelveDetailsServiceModel == null) return;

                Messenger.Default.Register<DismissUnshelveDetailsMessage>(this, DataContext, DismissUnshelveDetailsMessage);
            }
        }

        private void DismissUnshelveDetailsMessage(DismissUnshelveDetailsMessage dismissMessage)
        {
            var unshelveDetailsServiceModel = (UnshelveDetailsServiceModel)DataContext;
            DialogResult = dismissMessage.Success;
            Close();

            if (dismissMessage.DismissUnshelveModel)
                unshelveDetailsServiceModel.UnshelveServiceModel.Dismiss(dismissMessage.Success);
        }

        private void PendingChangeWorkItemCheckboxClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var unshelveDetailsServiceModel = (UnshelveDetailsServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedWorkItemModel = checkBox.DataContext as WorkItemModel;
            Debug.Assert(checkedWorkItemModel != null, "checkedWorkItemModel != null");

            var selectedWorkItemModels = WorkItemsListView.SelectedItems.Cast<WorkItemModel>().ToArray();

            var collection = selectedWorkItemModels.Contains(checkedWorkItemModel)
                                ? selectedWorkItemModels
                                : new[] { checkedWorkItemModel };

            Debug.Assert(checkBox.IsChecked != null, "checkBox.IsChecked != null");

            unshelveDetailsServiceModel.SelectWorkItemsCommand.Execute(new SelectWorkItemsCommandArgument()
            {
                Collection = collection,
                Value = checkBox.IsChecked.Value
            });
        }

        private void PendingChangesCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var unshelveDetailsServiceModel = (UnshelveDetailsServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedPendingChangeModel = checkBox.DataContext as PendingChangeModel;
            Debug.Assert(checkedPendingChangeModel != null, "pendingChangeModel != null");

            var selectedPendingChangeModels = PendingChangesListView.SelectedItems.Cast<PendingChangeModel>().ToArray();

            var collection = selectedPendingChangeModels.Contains(checkedPendingChangeModel)
                                ? selectedPendingChangeModels
                                : new[] { checkedPendingChangeModel };

            Debug.Assert(checkBox.IsChecked != null, "checkBox.IsChecked != null");

            unshelveDetailsServiceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument
            {
                Collection = collection,
                Value = checkBox.IsChecked.Value
            });
        }

        private void PendingChangesAllCheckboxOnClick(object sender, RoutedEventArgs e)
        {
            var unshelveDetailsServiceModel = (UnshelveDetailsServiceModel)DataContext;

            var checkAll =
                unshelveDetailsServiceModel.PendingChangesSummary == CollectionSelectionSummaryEnum.None;

            unshelveDetailsServiceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument()
            {
                Collection = unshelveDetailsServiceModel.PendingChanges.ToArray(),
                Value = checkAll
            });
        }
    }
}
