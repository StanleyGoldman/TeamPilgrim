using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio
{
    public class TeamFoundationHostWrapper : ITeamFoundationContextManager
    {
        private ITeamFoundationContextManager _teamFoundationHostObject;
        private Type _teamFoundationHostObjectType;
        private Lazy<MethodInfo> _promptForServerAndProjectsMethod;
        
        private static readonly Lazy<Type> ServerConnectingEventArgsType;
        private static readonly Lazy<Type> ServerConnectedEventArgsType;

        static TeamFoundationHostWrapper()
        {
            ServerConnectingEventArgsType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.ServerConnectingEventArgs, Microsoft.VisualStudio.TeamFoundation"));
            ServerConnectedEventArgsType = new Lazy<Type>(() => Type.GetType("Microsoft.VisualStudio.TeamFoundation.ServerConnectedEventArgs, Microsoft.VisualStudio.TeamFoundation"));
        }

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
            _promptForServerAndProjectsMethod = new Lazy<MethodInfo>(() => _teamFoundationHostObjectType.GetMethod("PromptForServerAndProjects", BindingFlags.Public | BindingFlags.Instance));

            var connectingEventInfo = _teamFoundationHostObjectType.GetEvent("Connecting", BindingFlags.Public | BindingFlags.Instance);
            var connectingEventHandlerConstructor = connectingEventInfo.EventHandlerType.GetConstructor(new[] {typeof (object), typeof (IntPtr)});
            var connectingEventHandler = (Delegate) connectingEventHandlerConstructor.Invoke(new object[]
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

// ReSharper disable UnusedMember.Local
        private void TeamFoundationHostConnecting(object serverConnectingEventArgs)
// ReSharper restore UnusedMember.Local
        {
            
        }

// ReSharper disable UnusedMember.Local
        private void TeamFoundationHostConnectionCompleted(object serverConnectedEventArgs)
// ReSharper restore UnusedMember.Local
        {
            
        }

        public DialogResult PromptForServerAndProjects(bool asynchronous = true)
        {
            return (DialogResult)_promptForServerAndProjectsMethod.Value.Invoke(_teamFoundationHostObject, new object[] { asynchronous });
        }

        public event EventHandler<ContextChangedEventArgs> ContextChanged;

        public event EventHandler<ContextChangingEventArgs> ContextChanging;

        public ITeamFoundationContext CurrentContext
        {
            get { return _teamFoundationHostObject.CurrentContext; }
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