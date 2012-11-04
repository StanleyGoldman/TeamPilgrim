using System.ComponentModel;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class BaseModel : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        protected readonly IPilgrimServiceModelProvider pilgrimServiceModelProvider;
        protected readonly ITeamPilgrimVsService teamPilgrimVsService;

        public BaseModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            this.pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            this.teamPilgrimVsService = teamPilgrimVsService;
        }

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
