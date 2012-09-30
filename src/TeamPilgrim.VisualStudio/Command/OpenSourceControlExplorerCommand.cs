using System.Windows.Input;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Command
{
    public class OpenSourceControlExplorerCommand : CommandModel
    {
        private readonly PilgrimProjectCollectionModel _pilgrimProjectCollectionModel;

        public OpenSourceControlExplorerCommand(PilgrimProjectCollectionModel pilgrimProjectCollectionModel)
        {
            _pilgrimProjectCollectionModel = pilgrimProjectCollectionModel;
        }

        public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}