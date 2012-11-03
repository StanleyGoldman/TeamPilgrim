using System;
using System.Reflection;
using System.Windows.Forms;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio
{
    public class TeamFoundationHostWrapper
    {
        private readonly dynamic _teamFoundationHostObject;
        private readonly Type _teamFoundationHostObjectType;
        private readonly Lazy<MethodInfo> _promptForServerAndProjectsMethod;

        public TeamFoundationHostWrapper(object teamFoundationHostObject)
        {
            _teamFoundationHostObject = teamFoundationHostObject;
            _teamFoundationHostObjectType = _teamFoundationHostObject.GetType();
            _promptForServerAndProjectsMethod = new Lazy<MethodInfo>(() => _teamFoundationHostObjectType.GetMethod("PromptForServerAndProjects", BindingFlags.Public | BindingFlags.Instance));
        }

        public DialogResult PromptForServerAndProjects(bool asynchronous = true)
        {
            return _promptForServerAndProjectsMethod.Value.Invoke(_teamFoundationHostObject, new object[] { asynchronous });
        }
    }
}