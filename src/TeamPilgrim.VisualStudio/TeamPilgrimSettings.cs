using System;
using EnvDTE;
using EnvDTE80;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    public class TeamPilgrimSettings
    {
        private readonly DTE2 _dte2;

        public TeamPilgrimSettings(DTE2 dte2)
        {
            _dte2 = dte2;
        }

        private Properties Properties
        {
            get
            {
                return _dte2.Properties["Team Pilgrim", "General"];
            }
        }

        public SelectedWorkItemCheckinActionEnum SelectedWorkItemCheckinAction
        {
            get
            {
                var property = Properties.Item("SelectedWorkItemCheckinAction");
                return (SelectedWorkItemCheckinActionEnum) Enum.Parse(typeof (SelectedWorkItemCheckinActionEnum), (string) property.Value);
            }
        }
    }
}