using System.Windows;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Dialogs
{

    public partial class LicenseDialog : Window
    {
        public LicenseDialog()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
