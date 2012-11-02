using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class BaseModel : INotifyPropertyChanged
    {
        public BaseModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private readonly Dispatcher _dispatcher;

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        #region Property Changed

        private PropertyChangedEventHandler _propertyChangedEvent;

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

        #endregion
    }
}
