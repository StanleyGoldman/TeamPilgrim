using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.VersionControl
{
    public class VersionControlPackageWrapper
    {
        private static readonly Lazy<Type> HatPackageType;
        private static readonly Lazy<FieldInfo> PackageInstanceField;
        private readonly object _hatPackageInstance;

        private static readonly Lazy<PropertyInfo> ResolveConflictsManagerProperty;
        private static readonly Lazy<Type> ResolveConflictsManagerType;
        private static readonly Lazy<MethodInfo> ResolveConflictsManagerResolveConflictsMethod;

        static VersionControlPackageWrapper()
        {
            HatPackageType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.VersionControl.HatPackage, Microsoft.VisualStudio.TeamFoundation.VersionControl"));
            PackageInstanceField = new Lazy<FieldInfo>(() => HatPackageType.Value.GetField("m_package", BindingFlags.Static | BindingFlags.NonPublic));
            ResolveConflictsManagerProperty = new Lazy<PropertyInfo>(() => HatPackageType.Value.GetProperty("ResolveConflictsManager", BindingFlags.Public | BindingFlags.Instance));
            ResolveConflictsManagerType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.VersionControl.ResolveConflictsManager, Microsoft.VisualStudio.TeamFoundation.VersionControl"));
            ResolveConflictsManagerResolveConflictsMethod = new Lazy<MethodInfo>(() => ResolveConflictsManagerType.Value.GetMethod("ResolveConflicts", BindingFlags.NonPublic | BindingFlags.Instance));
        }

        public VersionControlPackageWrapper()
        {
            _hatPackageInstance = PackageInstanceField.Value.GetValue(null);
        }

        private object ResolveConflictsManager
        {
            get
            {
                return ResolveConflictsManagerProperty.Value.GetMethod.Invoke(_hatPackageInstance, null);
            }
        }

        public void ResolveConflicts(Workspace workspace, string[] paths, bool recursive, bool afterCheckin)
        {
            ResolveConflictsManagerResolveConflictsMethod.Value.Invoke(ResolveConflictsManager, new object[] {workspace, paths, recursive, afterCheckin});
        }
    }
}