using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItem
{
    public class VersionControlPackageWrapper
    {
        private readonly Type _hatPackageType;
        private readonly object _hatPackageInstance;

        private readonly Lazy<PropertyInfo> _resolveConflictsManagerProperty;
        private readonly Lazy<Type> _resolveConflictsManagerType;
        private readonly Lazy<MethodInfo> _resolveConflictsManagerResolveConflictsMethod;

        public VersionControlPackageWrapper()
        {
            _hatPackageType = Type.GetType("Microsoft.VisualStudio.TeamFoundation.VersionControl.HatPackage, Microsoft.VisualStudio.TeamFoundation.VersionControl");

            Debug.Assert(_hatPackageType != null, "_hatPackageType != null");
            var instanceField = _hatPackageType.GetField("m_package", BindingFlags.Static | BindingFlags.NonPublic);

            Debug.Assert(instanceField != null, "instanceField != null");
            _hatPackageInstance = instanceField.GetValue(null);

            _resolveConflictsManagerProperty = new Lazy<PropertyInfo>(() => _hatPackageType.GetProperty("ResolveConflictsManager", BindingFlags.Public | BindingFlags.Instance));
            _resolveConflictsManagerType = new Lazy<Type>(() => ResolveConflictsManager.GetType());
            _resolveConflictsManagerResolveConflictsMethod = new Lazy<MethodInfo>(() => _resolveConflictsManagerType.Value.GetMethod("ResolveConflicts", BindingFlags.NonPublic | BindingFlags.Instance));
        }

        private object ResolveConflictsManager
        {
            get
            {
                return _resolveConflictsManagerProperty.Value.GetMethod.Invoke(_hatPackageInstance, null);
            }
        }

        public void ResolveConflicts(Workspace workspace, string[] paths, bool recursive, bool afterCheckin)
        {
            _resolveConflictsManagerResolveConflictsMethod.Value.Invoke(ResolveConflictsManager, new object[] {workspace, paths, recursive, afterCheckin});
        }
    }
}