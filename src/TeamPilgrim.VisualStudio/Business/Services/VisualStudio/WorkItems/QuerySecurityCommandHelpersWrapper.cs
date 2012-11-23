using System;
using System.Reflection;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItems
{
    //TODO: Make this and possible others that wrap static services static
    public class QuerySecurityCommandHelpersWrapper
    {
        private readonly Type _querySecurityCommandHelpersType;
        private readonly Lazy<MethodInfo> _handleSecurityCommandMethod;

        public QuerySecurityCommandHelpersWrapper()
        {
            _querySecurityCommandHelpersType = Type.GetType("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.QuerySecurityCommandHelpers, Microsoft.VisualStudio.TeamFoundation.WorkItemTracking");
            _handleSecurityCommandMethod = new Lazy<MethodInfo>(() => _querySecurityCommandHelpersType.GetMethod("HandleSecurityCommand", BindingFlags.Static | BindingFlags.Public));
        }

        public void HandleSecurityCommand(QueryItem queryItem)
        {
            _handleSecurityCommandMethod.Value.Invoke(null, new object[] {queryItem});
        }
    }
}
