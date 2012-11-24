using System;
using System.Reflection;
using System.Windows.Forms;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation
{
    public class TeamFoundationHostWrapper : ITeamFoundationHostWrapper
    {
        private readonly ITeamFoundationContextManager _teamFoundationHostObject;
        private readonly Type _teamFoundationHostObjectType;
        private readonly Lazy<FieldInfo> _commandHandlerField;
        private readonly Lazy<MethodInfo> _promptForServerAndProjectsMethod;

        public event EventHandler<ServerConnectedEventArgs> ServerConnected;
        public event EventHandler<ServerConnectingEventArgs> ServerConnecting;

        public event EventHandler<ContextChangedEventArgs> ContextChanged;
        public event EventHandler<ContextChangingEventArgs> ContextChanging;

        public TeamFoundationHostWrapper(ITeamFoundationContextManager teamFoundationHostObject)
        {
            _teamFoundationHostObject = teamFoundationHostObject;
            _teamFoundationHostObject.ContextChanged += delegate(object sender, ContextChangedEventArgs args)
                {
                    if (ContextChanged != null)
                        ContextChanged(sender, args);
                };

            _teamFoundationHostObject.ContextChanging += delegate(object sender, ContextChangingEventArgs args)
                {
                    if (ContextChanging != null)
                        ContextChanging(sender, args);
                };

            _teamFoundationHostObjectType = _teamFoundationHostObject.GetType();
            _commandHandlerField = new Lazy<FieldInfo>(() => _teamFoundationHostObjectType.GetField("m_commandHandler", BindingFlags.NonPublic | BindingFlags.Instance));
            _promptForServerAndProjectsMethod = new Lazy<MethodInfo>(() => _teamFoundationHostObjectType.GetMethod("PromptForServerAndProjects", BindingFlags.Public | BindingFlags.Instance));

            var connectingEventInfo = _teamFoundationHostObjectType.GetEvent("Connecting", BindingFlags.Public | BindingFlags.Instance);
            var connectingEventHandlerConstructor = connectingEventInfo.EventHandlerType.GetConstructor(new[] { typeof(object), typeof(IntPtr) });
            var connectingEventHandler = (Delegate)connectingEventHandlerConstructor.Invoke(new object[]
                {
                    this, typeof (TeamFoundationHostWrapper).GetMethod("TeamFoundationHostConnecting", BindingFlags.NonPublic | BindingFlags.Instance).MethodHandle.GetFunctionPointer()
                });

            connectingEventInfo.AddEventHandler(_teamFoundationHostObject, connectingEventHandler);

            var connectionCompletedEventInfo = _teamFoundationHostObjectType.GetEvent("ConnectionCompleted", BindingFlags.Public | BindingFlags.Instance);
            var connectionCompletedEventHandlerConstructor = connectionCompletedEventInfo.EventHandlerType.GetConstructor(new[] { typeof(object), typeof(IntPtr) });
            var connectionCompletedEventHandler = (Delegate)connectionCompletedEventHandlerConstructor.Invoke(new object[]
                {
                    this, typeof (TeamFoundationHostWrapper).GetMethod("TeamFoundationHostConnectionCompleted", BindingFlags.NonPublic | BindingFlags.Instance).MethodHandle.GetFunctionPointer()
                });

            connectionCompletedEventInfo.AddEventHandler(_teamFoundationHostObject, connectionCompletedEventHandler);
        }

        public ITeamFoundationContext CurrentContext
        {
            get { return _teamFoundationHostObject.CurrentContext; }
        }

        public CommandHandlerPackageWrapper CommandHandlerPackage
        {
            get
            {
                return new CommandHandlerPackageWrapper(_commandHandlerField.Value.GetValue(_teamFoundationHostObject));
            }
        }

        // ReSharper disable UnusedMember.Local
        private void TeamFoundationHostConnecting(object serverConnectingEventArgs)
        // ReSharper restore UnusedMember.Local
        {
            if (ServerConnecting == null)
                return;

            var serverConnectingEventArgsWrapper = new ServerConnectingEventArgs(serverConnectingEventArgs);
            ServerConnecting(this, serverConnectingEventArgsWrapper);
        }

        // ReSharper disable UnusedMember.Local
        private void TeamFoundationHostConnectionCompleted(object serverConnectedEventArgs)
        // ReSharper restore UnusedMember.Local
        {
            if (ServerConnected == null)
                return;

            var serverConnectedEventArgsWrapper = new ServerConnectedEventArgs(serverConnectedEventArgs);
            ServerConnected(this, serverConnectedEventArgsWrapper);
        }

        public DialogResult PromptForServerAndProjects(bool asynchronous = true)
        {
            return (DialogResult)_promptForServerAndProjectsMethod.Value.Invoke(_teamFoundationHostObject, new object[] { asynchronous });
        }

        public void SetContext(TfsTeamProjectCollection teamProjectCollection, string projectUri)
        {
            _teamFoundationHostObject.SetContext(teamProjectCollection, projectUri);
        }

        public void SetContext(TfsTeamProjectCollection teamProjectCollection, string projectUri, Guid teamId)
        {
            _teamFoundationHostObject.SetContext(teamProjectCollection, projectUri, teamId);
        }
    }
}