using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;

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

                Messenger.Default.Register<DismissMessage>(this, DataContext, dismissMessage =>
                    {
                        DialogResult = dismissMessage.Success;
                        Close();
                    });

                var unshelveDetailsServiceModel = (UnshelveDetailsServiceModel)value;
                if (unshelveDetailsServiceModel == null) return;
            }
        }

        private void PendingChangeWorkItemCheckboxClicked(object sender, RoutedEventArgs e)
        {
            
        }

        private void PendingChangesCheckboxClicked(object sender, RoutedEventArgs e)
        {
            
        }

        private void PendingChangesAllCheckboxOnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
