using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs
{
    /// <summary>
    /// Interaction logic for ShelveChangesDialog.xaml
    /// </summary>
    public partial class UnshelveDialog : Window
    {
        public UnshelveDialog()
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

                var unshelveDetailsServiceModel = (UnshelveServiceModel)value;
                if (unshelveDetailsServiceModel == null) return;
            }
        }
    }
}
