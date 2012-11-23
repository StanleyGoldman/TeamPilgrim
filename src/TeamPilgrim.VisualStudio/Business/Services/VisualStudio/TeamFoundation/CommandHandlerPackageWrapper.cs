using System;
using System.Reflection;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation
{
    public class CommandHandlerPackageWrapper
    {
        private static readonly Lazy<Type> CommandHandlerPackageType;
        private static readonly Lazy<MethodInfo> OpenSecuritySettingsMethod;

        static CommandHandlerPackageWrapper()
        {
            CommandHandlerPackageType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.CommandHandlerPackage, Microsoft.VisualStudio.TeamFoundation"));
            OpenSecuritySettingsMethod = new Lazy<MethodInfo>(() =>CommandHandlerPackageType.Value.GetMethod("OpenSecuritySettings",BindingFlags.Static | BindingFlags.Public));
        }

        public static void OpenSecuritySettings(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName, string projectUri)
        {
            OpenSecuritySettingsMethod.Value.Invoke(null, new object[] {tfsTeamProjectCollection, projectName, projectUri});
        }
    }
}
