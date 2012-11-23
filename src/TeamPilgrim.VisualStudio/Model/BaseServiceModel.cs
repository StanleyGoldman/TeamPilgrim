using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class BaseServiceModel : BaseModel
    {
        protected readonly ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider;
        protected readonly ITeamPilgrimVsService teamPilgrimVsService;

        public BaseServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
        {
            this.teamPilgrimServiceModelProvider = teamPilgrimServiceModelProvider;
            this.teamPilgrimVsService = teamPilgrimVsService;
        
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
        }

        #region Refresh Command

        public RelayCommand RefreshCommand { get; private set; }

        protected virtual void Refresh()
        {
            
        }

        protected virtual bool CanRefresh()
        {
            return false;
        }

        #endregion
    }
}
