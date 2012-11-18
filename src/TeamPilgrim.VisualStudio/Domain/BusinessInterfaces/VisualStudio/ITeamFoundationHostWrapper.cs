using System;
using System.Windows.Forms;
using JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.TeamFoundation;
using Microsoft.TeamFoundation.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio
{
    public interface ITeamFoundationHostWrapper : ITeamFoundationContextManager
    {
        event EventHandler<ServerConnectedEventArgs> ServerConnected;
        event EventHandler<ServerConnectingEventArgs> ServerConnecting;
        
        DialogResult PromptForServerAndProjects(bool asynchronous = true);
    }
}