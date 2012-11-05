using System.Collections.ObjectModel;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class WorkspaceModel : BaseModel
    {
        public ObservableCollection<PendingChange> PendingChanges { get; private set; }

        public Workspace Workspace { get; private set; }

        public WorkspaceModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, Workspace workspace)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            Workspace = workspace;
            var pendingChanges = workspace.GetPendingChanges();
            PendingChanges = new ObservableCollection<PendingChange>(pendingChanges);
        }
    }
}