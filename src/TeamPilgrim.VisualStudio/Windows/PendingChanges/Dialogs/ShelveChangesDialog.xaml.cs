using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

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
            
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            GridViewSort.ApplySort(PendingChangesListView.Items, "Change.LocalOrServerFolder", PendingChangesListView, (GridViewColumnHeader)GridViewColumnPendingChangesLocalOrServerFolder.Header, ListSortDirection.Ascending);
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

                Messenger.Default.Register<DismissMessage>(this, DataContext, dismissMessage =>
                {
                    DialogResult = dismissMessage.Success;
                    Close();
                });

                var shelvesetServiceModel = (ShelvesetServiceModel) value;
                if (shelvesetServiceModel == null) return;

                shelvesetServiceModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                    {
                        if (args.PropertyName == "EvaluatePoliciesAndCheckinNotes")
                        {
                            if (!shelvesetServiceModel.EvaluatePoliciesAndCheckinNotes)
                            {
                                if (PolicyWarningsRadioButton.IsChecked.HasValue && PolicyWarningsRadioButton.IsChecked.Value)
                                {
                                    SourceFilesRadioButton.IsChecked = true;
                                }
                            }
                        }
                    };

                shelvesetServiceModel.ShowPendingChangesItem += delegate(ShowPendingChangesTabItemEnum showPendingChangesTabItem)
                    {
                        switch (showPendingChangesTabItem)
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
                    };
            }
        }

        private void PendingChangesCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var shelvesetServiceModel = (ShelvesetServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedPendingChangeModel = checkBox.DataContext as PendingChangeModel;
            Debug.Assert(checkedPendingChangeModel != null, "pendingChangeModel != null");

            var selectedPendingChangeModels = PendingChangesListView.SelectedItems.Cast<PendingChangeModel>().ToArray();

            var collection = selectedPendingChangeModels.Contains(checkedPendingChangeModel)
                                ? selectedPendingChangeModels
                                : new[] { checkedPendingChangeModel };

            Debug.Assert(checkBox.IsChecked != null, "checkBox.IsChecked != null");

            shelvesetServiceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument
            {
                Collection = collection,
                Value = checkBox.IsChecked.Value
            });
        }

        private void PendingChangeWorkItemCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var shelvesetServiceModel = (ShelvesetServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedWorkItemModel = checkBox.DataContext as WorkItemModel;
            Debug.Assert(checkedWorkItemModel != null, "checkedWorkItemModel != null");

            var selectedWorkItemModels = WorkItemsListView.SelectedItems.Cast<WorkItemModel>().ToArray();

            var collection = selectedWorkItemModels.Contains(checkedWorkItemModel)
                    ? selectedWorkItemModels
                    : new[] { checkedWorkItemModel };

            Debug.Assert(checkBox.IsChecked != null, "checkBox.IsChecked != null");

            shelvesetServiceModel.SelectWorkItemsCommand.Execute(new SelectWorkItemsCommandArgument()
            {
                Collection = collection,
                Value = checkBox.IsChecked.Value
            });
        }

        private void PendingChangesAllCheckboxOnClick(object sender, RoutedEventArgs e)
        {
            var shelvesetServiceModel = (ShelvesetServiceModel)DataContext;

            var checkAll =
                shelvesetServiceModel.PendingChangesSummary == CollectionSelectionSummaryEnum.None;

            shelvesetServiceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument()
            {
                Collection = shelvesetServiceModel.PendingChanges.ToArray(),
                Value = checkAll
            });
        }
    }
}
