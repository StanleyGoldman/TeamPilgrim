using GalaSoft.MvvmLight.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems
{
    public interface IQueryItemCommandModel
    {
        RelayCommand<QueryDefinitionNode> OpenQueryDefinitionCommand { get; }
    }
}
