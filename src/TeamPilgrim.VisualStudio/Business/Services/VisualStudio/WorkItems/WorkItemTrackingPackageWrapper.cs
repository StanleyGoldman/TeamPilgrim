using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItems
{
    public class WorkItemTrackingPackageWrapper
    {
        private readonly Type _witPackageType;
        private readonly object _witPackageInstance;

        private readonly Lazy<MethodInfo> _openNewWorkItemMethod;
        private readonly Lazy<MethodInfo> _goToWorkItemMethod;
        private readonly Lazy<MethodInfo> _newQueryMethod;

        public WorkItemTrackingPackageWrapper()
        {
            _witPackageType = Type.GetType("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.WitPackage, Microsoft.VisualStudio.TeamFoundation.WorkItemTracking");

            Debug.Assert(_witPackageType != null, "_witPackageType != null");
            var instanceField = _witPackageType.GetField("m_package", BindingFlags.Static | BindingFlags.NonPublic);

            Debug.Assert(instanceField != null, "instanceField != null");
            _witPackageInstance = instanceField.GetValue(null);

            _openNewWorkItemMethod = new Lazy<MethodInfo>(() => _witPackageType.GetMethod("OpenNewWorkItem", BindingFlags.Public | BindingFlags.Instance));
            _newQueryMethod = new Lazy<MethodInfo>(() => _witPackageType.GetMethod("NewQuery", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(QueryFolder) }, null));
            _goToWorkItemMethod = new Lazy<MethodInfo>(() => _witPackageType.GetMethod("GoToWorkItem", BindingFlags.NonPublic | BindingFlags.Instance));
        }

        public void NewQuery(string projectName, QueryFolder parent)
        {
            _newQueryMethod.Value.Invoke(_witPackageInstance, new object[] { projectName, parent });
        }

        public void OpenNewWorkItem(TfsTeamProjectCollection tfs, string projectName, string typeName)
        {
            _openNewWorkItemMethod.Value.Invoke(_witPackageInstance, new object[] { tfs, projectName, typeName, null });
        }

        public void GoToWorkItem()
        {
            _goToWorkItemMethod.Value.Invoke(_witPackageInstance, null);
        }
    }
}
