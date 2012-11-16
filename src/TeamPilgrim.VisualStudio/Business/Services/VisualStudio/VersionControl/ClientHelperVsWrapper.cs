using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.VersionControl
{
    public class ClientHelperVsWrapper
    {
        private static readonly Lazy<Type> ClientHelperVsType;
        private static readonly Lazy<Type> ClientHelperType;
        private static readonly Lazy<PropertyInfo> InstanceProperty;
        private static readonly Lazy<MethodInfo> UndoMethod;
        private static readonly Lazy<MethodInfo> CompareChangesetChangesWithPreviousVersionsMethod;
        private static readonly Lazy<MethodInfo> CompareChangesetChangesWithLatestVersionsMethod;
        private static readonly Lazy<MethodInfo> CompareChangesetChangesWithWorkspaceVersionsMethod;

        private readonly object _clientHelperVsInstance;

        static ClientHelperVsWrapper()
        {
            ClientHelperVsType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.VersionControl.ClientHelperVS, Microsoft.VisualStudio.TeamFoundation.VersionControl"));
            InstanceProperty = new Lazy<PropertyInfo>(() => ClientHelperVsType.Value.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic));
            UndoMethod = new Lazy<MethodInfo>(() => ClientHelperVsType.Value.GetMethod("Undo", BindingFlags.Static | BindingFlags.NonPublic));
            CompareChangesetChangesWithPreviousVersionsMethod = new Lazy<MethodInfo>(() => ClientHelperVsType.Value.GetMethod("CompareChangesetChangesWithPreviousVersions", BindingFlags.Instance | BindingFlags.NonPublic));
            CompareChangesetChangesWithLatestVersionsMethod = new Lazy<MethodInfo>(() => ClientHelperVsType.Value.GetMethod("CompareChangesetChangesWithLatestVersions", BindingFlags.Instance | BindingFlags.NonPublic));
            CompareChangesetChangesWithWorkspaceVersionsMethod = new Lazy<MethodInfo>(() => ClientHelperVsType.Value.GetMethod("CompareChangesetChangesWithWorkspaceVersions", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        public ClientHelperVsWrapper()
        {
            _clientHelperVsInstance = InstanceProperty.Value.GetValue(null);
        }

        public void CompareChangesetChangesWithPreviousVersions(IList<PendingChange> pendingChanges)
        {
            CompareChangesetChangesWithPreviousVersionsMethod.Value.Invoke(_clientHelperVsInstance, new object[] { pendingChanges });
        }

        public void CompareChangesetChangesWithLatestVersions(IList<PendingChange> pendingChanges)
        {
            CompareChangesetChangesWithLatestVersionsMethod.Value.Invoke(_clientHelperVsInstance, new object[] { pendingChanges });
        }

        public void CompareChangesetChangesWithWorkspaceVersions(IList<PendingChange> pendingChanges, Workspace workspace)
        {
            CompareChangesetChangesWithWorkspaceVersionsMethod.Value.Invoke(_clientHelperVsInstance, new object[] { pendingChanges, workspace });
        }
    }


}