using System;
using System.Windows.Controls;
using System.Windows.Input;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class TeamPilgrimControl : UserControl
    {
        readonly RoutedCommand _myCommand = new RoutedCommand();
        private PilgrimProjectCollectionModel _pilgrimProjectCollectionModel;

        public TeamPilgrimControl()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(MyCommand, OnExecute, OnQueryCommandEnabled));
            DataContext = _pilgrimProjectCollectionModel = new PilgrimProjectCollectionModel(new PilgrimModelProvider());
        }

        public RoutedCommand MyCommand
        {
            get { return _myCommand; }
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