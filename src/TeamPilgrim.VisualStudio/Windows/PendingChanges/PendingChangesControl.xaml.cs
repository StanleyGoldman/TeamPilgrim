using System.Windows.Controls;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges
{
    /// <summary>
    /// Interaction logic for TeamPilgrimPendingChangesControl.xaml
    /// </summary>
    public partial class PendingChangesControl : UserControl
    {
        public PendingChangesControl()
        {
            InitializeComponent();
            DataContext = TeamPilgrimPackage.TeamPilgrimModel;
        }
    }
}
