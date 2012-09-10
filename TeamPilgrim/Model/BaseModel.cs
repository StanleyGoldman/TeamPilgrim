using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class BaseModel : INotifyPropertyChanged
    {
        public BaseModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        [Conditional("Debug")]
        protected void VerifyCalledOnUiThread()
        {
            Debug.Assert(Dispatcher.CurrentDispatcher == _dispatcher, "Call must be made on UI thread.");
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
                VerifyCalledOnUiThread();
                _propertyChangedEvent += value;
            }
            remove
            {
                VerifyCalledOnUiThread();
                _propertyChangedEvent -= value;
            }
        }

        protected void SendPropertyChanged(string propertyName)
        {
            VerifyCalledOnUiThread();
            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region State

        private ModelStateEnum _state;

        public ModelStateEnum State
        {
            get
            {
                VerifyCalledOnUiThread();
                return _state;
            }
            set
            {
                VerifyCalledOnUiThread();
                if (value != _state)
                {
                    _state = value;
                    SendPropertyChanged("State");
                }
            }
        }

        #endregion

        #region Active

        private bool _isActive;

        public bool IsActive
        {
            get
            {
                VerifyCalledOnUiThread();
                return _isActive;
            }

            private set
            {
                VerifyCalledOnUiThread();
                if (value != _isActive)
                {
                    _isActive = value;
                    SendPropertyChanged("IsActive");
                }
            }
        }

        public void Activate()
        {
            VerifyCalledOnUiThread();
            if (_isActive) return;

            IsActive = true;
            OnActivated();
        }

        public void Deactivate()
        {
            VerifyCalledOnUiThread();
            if (!_isActive) return;

            IsActive = false;
            OnDeactivated();
        }

        protected virtual void OnActivated()
        {
        }

        protected virtual void OnDeactivated()
        {
        }

        #endregion
    }
}
