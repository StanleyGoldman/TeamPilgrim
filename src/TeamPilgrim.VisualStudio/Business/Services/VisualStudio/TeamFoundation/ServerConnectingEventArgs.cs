using System;
using System.Reflection;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation
{
    public class ServerConnectingEventArgs
    {
        private static readonly Lazy<Type> ServerConnectingEventArgsType;
        private static readonly Lazy<PropertyInfo> TeamProjectCollectionPropertyInfo;

        static ServerConnectingEventArgs()
        {
            ServerConnectingEventArgsType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.ServerConnectingEventArgs, Microsoft.VisualStudio.TeamFoundation"));
            TeamProjectCollectionPropertyInfo = new Lazy<PropertyInfo>(() => ServerConnectingEventArgsType.Value.GetProperty("TeamProjectCollection"));
        }

        private readonly object _instance;

        public ServerConnectingEventArgs(object instance)
        {
            _instance = instance;
        }

        public TfsTeamProjectCollection TeamProjectCollection
        {
            get { return (TfsTeamProjectCollection)TeamProjectCollectionPropertyInfo.Value.GetValue(_instance); }
        }
    }
}
