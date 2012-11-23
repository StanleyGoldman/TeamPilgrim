using System;
using System.Reflection;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItems
{
    public static class OpenWorkItemHelperWrapper
    {
        private static readonly Lazy<Type> OpenWorkItemHelperType;
        private static readonly Lazy<MethodInfo> OpenWorkItemMethodByWorkItemIdAndType;
        private static readonly Lazy<MethodInfo> OpenWorkItemMethodByWorkItem;

        static OpenWorkItemHelperWrapper()
        {
            OpenWorkItemHelperType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.OpenWorkItemHelper, Microsoft.VisualStudio.TeamFoundation.WorkItemTracking"));
            OpenWorkItemMethodByWorkItemIdAndType = new Lazy<MethodInfo>(() => OpenWorkItemHelperType.Value.GetMethod("OpenWorkItem", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(int), typeof(WorkItemType), typeof(bool) }, null));
            OpenWorkItemMethodByWorkItem = new Lazy<MethodInfo>(() => OpenWorkItemHelperType.Value.GetMethod("OpenWorkItem", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(WorkItem), typeof(bool) }, null));
        }

        public static void OpenWorkItem(int id, WorkItemType workItemType, bool useProvisionalTab)
        {
            OpenWorkItemMethodByWorkItemIdAndType.Value.Invoke(null, new object[] { id, workItemType, useProvisionalTab });
        }

        public static void OpenWorkItem(WorkItem workItem, bool useProvisionalTab)
        {
            OpenWorkItemMethodByWorkItem.Value.Invoke(null, new object[] { workItem, useProvisionalTab });
        }
    }
}
