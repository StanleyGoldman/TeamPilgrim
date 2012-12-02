using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using Microsoft.VisualStudio.Shell;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Settings
{
    [CLSCompliant(false), ComVisible(true)]
    public class SettingsPage : DialogPage
    {
        private readonly SettingsControl _settingsControl;

        protected override IWin32Window Window
        {
            get
            {
                return _settingsControl;
            }
        }

        public SettingsPage()
        {
            _settingsControl = new SettingsControl();
        }

        public override void LoadSettingsFromStorage()
        {
            _settingsControl.SelectedWorkItemCheckinAction = TeamPilgrimPackage.TeamPilgrimSettings.SelectedWorkItemCheckinAction;
            _settingsControl.PreserveShelvedChangesLocally = TeamPilgrimPackage.TeamPilgrimSettings.PreserveShelvedChangesLocally;
        }

        public override void SaveSettingsToStorage()
        {
            TeamPilgrimPackage.TeamPilgrimSettings.SelectedWorkItemCheckinAction = _settingsControl.SelectedWorkItemCheckinAction;
            TeamPilgrimPackage.TeamPilgrimSettings.PreserveShelvedChangesLocally = _settingsControl.PreserveShelvedChangesLocally;
            TeamPilgrimPackage.TeamPilgrimSettings.Save();
        }
    }
}