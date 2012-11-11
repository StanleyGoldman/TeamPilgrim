﻿using System.Windows;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectWorkItemQueryControl.xaml
    /// </summary>
    public partial class PolicyFailureDialog : Window
    {
        public PolicyFailureDialog()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
