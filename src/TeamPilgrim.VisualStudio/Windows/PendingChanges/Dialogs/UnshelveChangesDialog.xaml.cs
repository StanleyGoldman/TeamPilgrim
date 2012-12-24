using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties;

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
    }
}
