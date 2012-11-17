using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio
{
    public class TeamFoundationHostWrapper : ITeamFoundationContextManager
    {
        private readonly ITeamFoundationContextManager _teamFoundationHostObject;
        private readonly Type _teamFoundationHostObjectType;
        private readonly Lazy<MethodInfo> _promptForServerAndProjectsMethod;

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
        }

        public DialogResult PromptForServerAndProjects(bool asynchronous = true)
        {
            return (DialogResult) _promptForServerAndProjectsMethod.Value.Invoke(_teamFoundationHostObject, new object[] { asynchronous });
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