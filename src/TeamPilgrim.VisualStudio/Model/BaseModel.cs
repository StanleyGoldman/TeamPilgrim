using System.ComponentModel;
using System.Windows.Threading;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class BaseModel : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private PropertyChangedEventHandler _propertyChangedEvent;

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChangedEvent += value;
            }
            remove
            {
                _propertyChangedEvent -= value;
            }
        }

        protected void SendPropertyChanged(string propertyName)
        {
            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}