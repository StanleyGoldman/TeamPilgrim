using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.BuildDefinitions
{
    public interface IBuildDefinitionCommandModel
    {
        RelayCommand<BuildDefinitionModel> OpenBuildDefintionCommand { get; set; }
        RelayCommand<BuildDefinitionModel> ViewBuildsCommand { get; set; }
    }
}