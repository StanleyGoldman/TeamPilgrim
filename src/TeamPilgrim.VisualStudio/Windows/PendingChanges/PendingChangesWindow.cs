using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.PendingChanges
{
    [Guid("9BC80FB7-4DCF-446A-94B1-DE9E828BCB67")]
    public class PendingChangesWindow : ToolWindowPane
    {
        public PendingChangesWindow()
            : base(null)
        {
            this.Caption = Resources.PendingChangesToolWindowTitle;

            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            var pendingChangesControl = new PendingChangesControl();
            base.Content = pendingChangesControl;
            
            pendingChangesControl.DataContext = TeamPilgrimPackage.TeamPilgrimModel;
        }
    }
}