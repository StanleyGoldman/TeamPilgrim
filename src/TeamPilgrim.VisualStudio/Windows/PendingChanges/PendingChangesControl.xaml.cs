using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs;

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

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            GridViewSort.ApplySort(PendingChangesListView.Items, "Change.LocalOrServerFolder", PendingChangesListView, (GridViewColumnHeader)GridViewColumnPendingChangesLocalOrServerFolder.Header);
        }

        public new object DataContext
        {
            get { return base.DataContext; }
            set
            {
                base.DataContext = value;

                var teamPilgrimServiceModel = (TeamPilgrimServiceModel)value;
                if (teamPilgrimServiceModel == null) return;

                teamPilgrimServiceModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                    {
                        if (args.PropertyName == "SelectedWorkspaceModel")
                        {
                            AttachShowPendingChangesItemEvent(teamPilgrimServiceModel);
                        }
                    };

                AttachShowPendingChangesItemEvent(teamPilgrimServiceModel);
            }
        }

        private void AttachShowPendingChangesItemEvent(TeamPilgrimServiceModel teamPilgrimServiceModel)
        {
            if (teamPilgrimServiceModel.SelectedWorkspaceModel == null) return;

            teamPilgrimServiceModel.SelectedWorkspaceModel.ShowPendingChangesItem += SelectedWorkspaceModelOnShowPendingChangesItem;
            teamPilgrimServiceModel.SelectedWorkspaceModel.ShowShelveDialog += SelectedWorkspaceModelOnShowShelveDialog;
            teamPilgrimServiceModel.SelectedWorkspaceModel.ShowUnshelveDialog += SelectedWorkspaceModelOnShowUnshelveDialog;
        }

        private void SelectedWorkspaceModelOnShowUnshelveDialog()
        {
            var unshelveChangesDialog = new UnshelveChangesDialog
            {
                DataContext = DataContext
            };

            var dialogResult = unshelveChangesDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {

            }
        }

        private void SelectedWorkspaceModelOnShowShelveDialog(ShelvesetServiceModel model)
        {
            var shelveChangesDialog = new ShelveChangesDialog
                {
                    DataContext = model
                };

            var dialogResult = shelveChangesDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
            }
        }

        private void SelectedWorkspaceModelOnShowPendingChangesItem(ShowPendingChangesTabItemEnum showPendingChangesTabItemEnum)
        {
            switch (showPendingChangesTabItemEnum)
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
        }

        private void PendingChangeWorkItemCheckboxClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var teamPilgrimModel = (TeamPilgrimServiceModel)DataContext;

            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, "checkBox != null");

            var checkedWorkItemModel = checkBox.DataContext as WorkItemModel;
            Debug.Assert(checkedWorkItemModel != null, "checkedWorkItemModel != null");

            var selectedWorkItemModels = WorkItemsListView.SelectedItems.Cast<WorkItemModel>().ToArray();

            var collection = selectedWorkItemModels.Contains(checkedWorkItemModel)
                                ? selectedWorkItemModels
                                : new[] { checkedWorkItemModel };

            Debug.Assert(checkBox.IsChecked != null, "checkBox.IsChecked != null");

            teamPilgrimModel.SelectedWorkspaceModel.SelectWorkItemsCommand.Execute(new SelectWorkItemsCommandArgument()
            {
                Collection = collection,
                Value = checkBox.IsChecked.Value
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

            var collection = selectedPendingChangeModels.Contains(checkedPendingChangeModel)
                                ? selectedPendingChangeModels
                                : new[] { checkedPendingChangeModel };

            Debug.Assert(checkBox.IsChecked != null, "checkBox.IsChecked != null");

            teamPilgrimModel.SelectedWorkspaceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument
            {
                Collection = collection,
                Value = checkBox.IsChecked.Value
            });
        }

        private void PendingChangesAllCheckboxOnClick(object sender, RoutedEventArgs e)
        {
            var teamPilgrimModel = (TeamPilgrimServiceModel)DataContext;
            var selectedWorkspaceModel = teamPilgrimModel.SelectedWorkspaceModel;

            var checkAll =
                selectedWorkspaceModel.PendingChangesSummary == CollectionSelectionSummaryEnum.None;

            selectedWorkspaceModel.SelectPendingChangesCommand.Execute(new SelectPendingChangesCommandArgument()
            {
                Collection = selectedWorkspaceModel.PendingChanges.ToArray(),
                Value = checkAll
            });
        }
    }
}
