using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.BuildDefinitions
{
    public interface IBuildDefinitionCommandModel
    {
        RelayCommand NewBuildDefinitionCommand { get; set; }
        RelayCommand<BuildDefinitionModel> DeleteBuildDefinitionCommand { get; set; }
        RelayCommand<BuildDefinitionModel> OpenBuildDefintionCommand { get; set; }
        RelayCommand<BuildDefinitionModel> CloneBuildDefinitionCommand { get; set; }
        RelayCommand<BuildDefinitionModel> OpenProcessFileLocationCommand { get; set; }
        RelayCommand<BuildDefinitionModel> ViewBuildsCommand { get; set; }
        RelayCommand<BuildDefinitionModel> QueueBuildCommand { get; set; }
    }
}