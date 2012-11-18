using System;
using System.Reflection;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation
{
    public class ServerConnectedEventArgs
    {
        private static readonly Lazy<Type> ServerConnectedEventArgsType;
        private static readonly Lazy<PropertyInfo> TeamProjectCollectionPropertyInfo;
        private static readonly Lazy<PropertyInfo> ErrorPropertyInfo;
        private static readonly Lazy<PropertyInfo> StatusPropertyInfo;

        static ServerConnectedEventArgs()
        {
            ServerConnectedEventArgsType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.ServerConnectedEventArgs, Microsoft.VisualStudio.TeamFoundation"));
            TeamProjectCollectionPropertyInfo = new Lazy<PropertyInfo>(() => ServerConnectedEventArgsType.Value.GetProperty("TeamProjectCollection"));
            ErrorPropertyInfo = new Lazy<PropertyInfo>(() => ServerConnectedEventArgsType.Value.GetProperty("Error"));
            StatusPropertyInfo = new Lazy<PropertyInfo>(() => ServerConnectedEventArgsType.Value.GetProperty("Status"));
        }

        private readonly object _instance;

        public ServerConnectedEventArgs(object instance)
        {
            _instance = instance;
        }

        public TfsTeamProjectCollection TeamProjectCollection
        {
            get { return (TfsTeamProjectCollection)TeamProjectCollectionPropertyInfo.Value.GetValue(_instance); }
        }

        public Exception Error
        {
            get { return (Exception)ErrorPropertyInfo.Value.GetValue(_instance); }
        }

        public CompletionStatusEnum Status
        {
            get
            {
                var value = StatusPropertyInfo.Value.GetValue(_instance);
                var s = value.ToString();

                return (CompletionStatusEnum) Enum.Parse(typeof (CompletionStatusEnum), s);
            }
        }

        public enum CompletionStatusEnum
        {
            Connected,
            Failed,
            Cancelled,
        }
    }
}