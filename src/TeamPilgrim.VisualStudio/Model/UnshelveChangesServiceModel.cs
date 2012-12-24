using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Core;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class UnshelveChangesServiceModel : BaseServiceModel
    {
        public ProjectCollectionServiceModel ProjectCollectionServiceModel { get; private set; }
        public WorkspaceServiceModel WorkspaceServiceModel { get; private set; }

        private string _shelvesetOwner;
        public string ShelvesetOwner
        {
            get
            {
                return _shelvesetOwner;
            }
            private set
            {
                if (_shelvesetOwner == value) return;

                _shelvesetOwner = value;

                SendPropertyChanged("ShelvesetOwner");
            }
        }

        public UnshelveChangesServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, 
                                           ITeamPilgrimVsService teamPilgrimVsService,
                                           ProjectCollectionServiceModel projectCollectionServiceModel, WorkspaceServiceModel workspaceServiceModel)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectCollectionServiceModel = projectCollectionServiceModel;
            WorkspaceServiceModel = workspaceServiceModel;

            ShelvesetOwner = projectCollectionServiceModel.TfsTeamProjectCollection.AuthorizedIdentity.UniqueName;
        }
    }
}
