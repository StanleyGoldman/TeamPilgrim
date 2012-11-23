using System;
using System.Reflection;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation
{
    public class CommandHandlerPackageWrapper
    {
        private static readonly Lazy<Type> CommandHandlerPackageType;
        private static readonly Lazy<MethodInfo> OpenSecuritySettingsMethod;
        private static readonly Lazy<MethodInfo> OpenGroupMembershipMethod;
        private static readonly Lazy<MethodInfo> OpenProjectAlertsMethod;
        private static readonly Lazy<MethodInfo> OpenWorkItemsClassificationMethod;

        static CommandHandlerPackageWrapper()
        {
            CommandHandlerPackageType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.CommandHandlerPackage, Microsoft.VisualStudio.TeamFoundation"));
            OpenSecuritySettingsMethod = new Lazy<MethodInfo>(() => CommandHandlerPackageType.Value.GetMethod("OpenSecuritySettings", BindingFlags.Static | BindingFlags.Public));
            OpenProjectAlertsMethod = new Lazy<MethodInfo>(() => CommandHandlerPackageType.Value.GetMethod("OpenProjectAlerts", BindingFlags.Static | BindingFlags.Public));
            OpenGroupMembershipMethod = new Lazy<MethodInfo>(() => CommandHandlerPackageType.Value.GetMethod("OpenGroupMembership", BindingFlags.Static | BindingFlags.Public));
            OpenWorkItemsClassificationMethod = new Lazy<MethodInfo>(() => CommandHandlerPackageType.Value.GetMethod("OpenWorkItemsClassification", BindingFlags.Static | BindingFlags.Public));
        }

        public static void OpenWorkItemsClassification(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName, string serviceType, Guid serviceIdentifier)
        {
            OpenGroupMembershipMethod.Value.Invoke(null, new object[] { tfsTeamProjectCollection, projectName, serviceType, serviceIdentifier });
        }

        public static void OpenWorkItemsAreas(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName)
        {
            OpenWorkItemsClassification(tfsTeamProjectCollection, projectName, "AreasManagementWeb", FrameworkServiceIdentifiers.AreasManagementWeb);
        }
        
        public static void OpenWorkItemsIterations(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName)
        {
            OpenWorkItemsClassification(tfsTeamProjectCollection, projectName, "IterationsManagementWeb", FrameworkServiceIdentifiers.IterationsManagementWeb);
        }

        public static void OpenGroupMembership(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName, string projectUri)
        {
            OpenGroupMembershipMethod.Value.Invoke(null, new object[] { tfsTeamProjectCollection, projectName, projectUri });
        }

        public static void OpenSecuritySettings(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName, string projectUri)
        {
            OpenSecuritySettingsMethod.Value.Invoke(null, new object[] { tfsTeamProjectCollection, projectName, projectUri });
        }

        public static void OpenProjectAlerts(TfsTeamProjectCollection tfsTeamProjectCollection, string projectName)
        {
            OpenProjectAlertsMethod.Value.Invoke(null, new object[] { tfsTeamProjectCollection, projectName });
        }
    }
}
