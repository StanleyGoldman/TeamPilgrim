using System;
using System.Reflection;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItems
{
    public class WitDefaultCommandHandlerWrapper
    {
        private static readonly Type WitDefaultCommandHandlerType;
        private static readonly Lazy<MethodInfo> ViewQueryMethod;

        static WitDefaultCommandHandlerWrapper()
        {
            WitDefaultCommandHandlerType = Type.GetType("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.WitDefaultCommandHandler, Microsoft.VisualStudio.TeamFoundation.WorkItemTracking");
            ViewQueryMethod = new Lazy<MethodInfo>(() => WitDefaultCommandHandlerType.GetMethod("ViewQuery", BindingFlags.Static | BindingFlags.Public));
        }

        public static void ViewQuery(QueryDefinition queryDefinition)
        {
            ViewQueryMethod.Value.Invoke(null, new object[] { queryDefinition });
        }

        private readonly object _witDefaultCommandHandlerInstance;

        public WitDefaultCommandHandlerWrapper(object witDefaultCommandHandlerInstance)
        {
            _witDefaultCommandHandlerInstance = witDefaultCommandHandlerInstance;
        }
    }
}