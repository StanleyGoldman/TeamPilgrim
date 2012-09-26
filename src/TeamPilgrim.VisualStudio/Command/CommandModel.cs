using System.Windows.Input;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Command
{
    //http://blogs.msdn.com/b/dancre/archive/2006/08/05/689542.aspx

    public abstract class CommandModel
    {
        private readonly RoutedCommand _routedCommand;

        protected CommandModel()
        {
            _routedCommand = new RoutedCommand();
        }

        public RoutedCommand Command
        {
            get { return _routedCommand; }
        }

        public virtual void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        public abstract void OnExecute(object sender, ExecutedRoutedEventArgs e);
    }
}
