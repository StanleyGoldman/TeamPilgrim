using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Explorer
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        static TreeViewItem VisualUpwardSearchForTreeViewItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        public ExplorerControl()
        {
            InitializeComponent();
            DataContext = TeamPilgrimPackage.TeamPilgrimServiceModel;
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearchForTreeViewItem(e.OriginalSource as DependencyObject);

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