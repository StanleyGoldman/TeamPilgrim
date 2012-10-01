using System;
using System.Windows.Controls;
using System.Windows.Input;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectNodes;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class TeamPilgrimControl : UserControl
    {
        private readonly PilgrimModel _pilgrimModel;

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

        private void treeview_mouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeView = (TreeView) sender;
            var selectedItem = treeView.SelectedItem;

            var sourceControlNode = selectedItem as SourceControlNode;
            if(sourceControlNode != null)
            {
                VersionControlExplorerExt versionControlExplorerExt = TeamPilgrimPackage.VersionControlExt.Explorer;
                
                //Wrong
                var serverPath = sourceControlNode.PilgrimProjectModel.Path.ToString();

                var path = "$/WiseMarkit/Design";

                versionControlExplorerExt.Navigate(path);
            }
        }
    }
}