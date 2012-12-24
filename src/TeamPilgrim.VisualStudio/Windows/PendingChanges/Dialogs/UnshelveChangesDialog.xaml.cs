using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs
{
    /// <summary>
    /// Interaction logic for ShelveChangesDialog.xaml
    /// </summary>
    public partial class UnshelveChangesDialog : Window
    {
        public UnshelveChangesDialog()
        {
            InitializeComponent();
            
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            GridViewSort.ApplySort(PendingSetsListView.Items, "Shelveset.CreationDate", PendingSetsListView, (GridViewColumnHeader)GridViewColumnPendingSetsShelvesetCreationDate.Header, ListSortDirection.Descending);
        }

        public new object DataContext
        {
            get { return base.DataContext; }
            set
            {
                base.DataContext = value;

                var unshelveChangesServiceModel = (UnshelveChangesServiceModel)value;
                if (unshelveChangesServiceModel == null) return;

                unshelveChangesServiceModel.Dismiss += delegate(bool success)
                {
                    DialogResult = success;
                    Close();
                };
            }
        }
    }
}
