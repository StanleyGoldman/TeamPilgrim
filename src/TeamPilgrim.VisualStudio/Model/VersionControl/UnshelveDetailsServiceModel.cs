using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Messages;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class UnshelveDetailsServiceModel : BaseServiceModel
    {
        public Shelveset Shelveset { get; private set; }

        public UnshelveDetailsServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, Shelveset shelveset)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            Shelveset = shelveset;
         
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        #region Cancel Command

        public RelayCommand CancelCommand { get; private set; }

        public void Cancel()
        {
            Messenger.Default.Send(new DismissMessage { Success = false }, this);
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion
    }
}
