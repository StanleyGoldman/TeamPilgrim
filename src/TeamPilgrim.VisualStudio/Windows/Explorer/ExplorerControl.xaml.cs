using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Explorer
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        public ExplorerControl()
        {
            //NOTE: Only enable this when you are looking to debug a particular issue
            //Certain Visual Studio dialogs like the "Work Item Query" can be expected to throw binding errors
            //BindingErrorTraceListener.SetTrace();

            InitializeComponent();
            DataContext = TeamPilgrimPackage.TeamPilgrimModel;
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearchForTreeViewItem(e.OriginalSource as DependencyObject);

            if (treeViewItem == null) return;

            treeViewItem.IsSelected = true;
            e.Handled = true;
        }

        static TreeViewItem VisualUpwardSearchForTreeViewItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}