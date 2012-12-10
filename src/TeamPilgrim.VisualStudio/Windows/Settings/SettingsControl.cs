using System.Collections.Generic;
using System.Windows.Forms;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Windows.Settings
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();

            var selectedWorkItemCheckinActionEnums = new Dictionary<string, SelectedWorkItemCheckinActionEnum>
                {
                    {SelectedWorkItemCheckinActionEnum.Associate.ToString(), SelectedWorkItemCheckinActionEnum.Associate},
                    {SelectedWorkItemCheckinActionEnum.Resolve.ToString(), SelectedWorkItemCheckinActionEnum.Resolve}
                };

            cmbDefaultWorkItemAssociation.DisplayMember = "Key";
            cmbDefaultWorkItemAssociation.ValueMember = "Value";
            cmbDefaultWorkItemAssociation.DataSource = new BindingSource(selectedWorkItemCheckinActionEnums, null);
            cmbDefaultWorkItemAssociation.SelectedItem = SelectedWorkItemCheckinActionEnum.Resolve;
        }

        public SelectedWorkItemCheckinActionEnum SelectedWorkItemCheckinAction
        {
            get { return (SelectedWorkItemCheckinActionEnum)cmbDefaultWorkItemAssociation.SelectedValue; }
            set { cmbDefaultWorkItemAssociation.SelectedValue = value; }
        }

        public bool PreserveShelvedChangesLocally
        {
            get { return chkPreservePendingChangesLocally.Checked; }
            set { chkPreservePendingChangesLocally.Checked = value; }
        }

        public bool EvaluatePoliciesDuringShelve
        {
            get { return chkEvaluatePolicies.Checked; }
            set { chkEvaluatePolicies.Checked = value; }
        }
    }
}
