using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public interface IPendingChangeCommandModel
    {
        RelayCommand<PendingChangeModel> ViewPendingChangeCommand { get; }
    }
}
