using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project;
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
    }

    public class TeamPilgrilViewStyleSelector : StyleSelector
    {
        public Style SourceControlNodeStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var sourceControlNode = item as SourceControlNode;
            if (sourceControlNode != null)
            {
                return SourceControlNodeStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}