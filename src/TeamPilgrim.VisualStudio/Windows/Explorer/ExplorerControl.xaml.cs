using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Explorer
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        public ExplorerControl()
        {
            InitializeComponent();
            DataContext = TeamPilgrimPackage.TeamPilgrimServiceModel;
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualTreeHelpers.FindAncestor<TreeViewItem>((DependencyObject) e.OriginalSource);

            if (treeViewItem == null) return;

            treeViewItem.IsSelected = true;
            e.Handled = true;
        }

        private void OnProjectTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            var treeViewItem = (TreeViewItem) sender;
            var selectedProjectServiceModel = (ProjectServiceModel) treeViewItem.DataContext;

            TeamPilgrimPackage.TeamPilgrimServiceModel.ActiveProjectCollectionModel.SetActiveProjectCommand.Execute(selectedProjectServiceModel);
        }
    }
}