using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.VersionControl
{
    public class PendingChangesPageViewModelUtilsWrapper
    {
        private static readonly Lazy<Type> PendingChangesPageViewModelUtilsType;
        private static readonly Lazy<PropertyInfo> InstanceProperty;
        private static readonly Lazy<MethodInfo> UndoChangesMethod;
        private static readonly Lazy<MethodInfo> ViewMethod;

        private readonly object _pendingChangesPageViewModelUtilsWrapperInstance;

        static PendingChangesPageViewModelUtilsWrapper()
        {
            PendingChangesPageViewModelUtilsType = new Lazy<Type>(() => Type.GetType("Microsoft.TeamFoundation.VersionControl.Controls.PendingChanges.PendingChangesPageViewModelUtils, Microsoft.TeamFoundation.VersionControl.Controls"));
            InstanceProperty = new Lazy<PropertyInfo>(() => PendingChangesPageViewModelUtilsType.Value.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic));
            ViewMethod = new Lazy<MethodInfo>(() => PendingChangesPageViewModelUtilsType.Value.GetMethod("View", BindingFlags.Instance | BindingFlags.NonPublic));
            UndoChangesMethod = new Lazy<MethodInfo>(() => PendingChangesPageViewModelUtilsType.Value.GetMethod("UndoChanges", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        public PendingChangesPageViewModelUtilsWrapper()
        {
            _pendingChangesPageViewModelUtilsWrapperInstance = InstanceProperty.Value.GetValue(null);
        }

        public void UndoChanges(Workspace workspace, IList<PendingChange> pendingChanges)
        {
            UndoChangesMethod.Value.Invoke(_pendingChangesPageViewModelUtilsWrapperInstance, new object[] { workspace, pendingChanges });
        }

        public void View(Workspace workspace, IList<PendingChange> pendingChanges)
        {
            ViewMethod.Value.Invoke(_pendingChangesPageViewModelUtilsWrapperInstance, new object[] { workspace, pendingChanges });
        }
    }
}