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

        [Category("General")]
        public string SelectedWorkItemCheckinAction
        {
            get
            {
                var defaultSelectedWorkItemCheckinAction = _settingsControl.SelectedWorkItemCheckinAction;
                return defaultSelectedWorkItemCheckinAction.ToString();
            }
            set
            {
                SelectedWorkItemCheckinActionEnum result;
                if (Enum.TryParse(value, true, out result))
                {
                    _settingsControl.SelectedWorkItemCheckinAction = result;
                }
                else
                {
                    _settingsControl.SelectedWorkItemCheckinAction = SelectedWorkItemCheckinActionEnum.Resolve;
                }
            }
        }

        public SettingsPage()
        {
            _settingsControl = new SettingsControl();
        }

        protected override IWin32Window Window
        {
            get
            {
                return _settingsControl;
            }
        }

    }
}