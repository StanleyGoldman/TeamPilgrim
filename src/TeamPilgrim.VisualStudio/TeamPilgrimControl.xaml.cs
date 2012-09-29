using System;
using System.Windows.Controls;
using System.Windows.Input;
using JustAProgrammer.TeamPilgrim.VisualStudio.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class TeamPilgrimControl : UserControl
    {
        private PilgrimModel _pilgrimModel;

        public TeamPilgrimControl()
        {
            BindingErrorTraceListener.SetTrace(); 

            InitializeComponent();
            DataContext = _pilgrimModel = new PilgrimModel(new PilgrimServiceServiceModelProvider());
        }

        private void OnQueryCommandEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(e.Parameter as string);
            e.Handled = true;
        }

        private void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
        }

    }
}