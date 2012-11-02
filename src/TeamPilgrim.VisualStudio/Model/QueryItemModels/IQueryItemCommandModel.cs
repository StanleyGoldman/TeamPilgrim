using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels
{
    public interface IQueryItemCommandModel
    {
        RelayCommand<QueryDefinitionModel> OpenQueryDefinitionCommand { get; }
        RelayCommand<QueryDefinitionModel> EditQueryDefinitionCommand { get; }
        RelayCommand<QueryDefinitionModel> DeleteQueryDefinitionCommand { get; }
    }
}
