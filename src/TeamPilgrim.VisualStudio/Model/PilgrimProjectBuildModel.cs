using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectBuildModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly IPilgrimBuildServiceModelProvider _buildServiceModelProvider;
        private readonly PilgrimProjectCollection _collection;
        private readonly PilgrimProject _project;

        public PilgrimProjectBuildModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, IPilgrimBuildServiceModelProvider buildServiceModelProvider, PilgrimProjectCollection collection, PilgrimProject project)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            _buildServiceModelProvider = buildServiceModelProvider;
            _collection = collection;
            _project = project;

            State = ModelStateEnum.Invalid;
        }

        protected override void OnActivated()
        {
            VerifyCalledOnUiThread();

            if (ThreadPool.QueueUserWorkItem(PopulatePilgrimBuildModelCallback))
            {
                State = ModelStateEnum.Fetching;
            }
        }

        private void PopulatePilgrimBuildModelCallback(object state)
        {
            PilgrimBuildDetail[] pilgrimBuildDetails;
            if (_buildServiceModelProvider.TryGetBuildsByProjectName(out pilgrimBuildDetails, _project.Project.Name))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        PilgrimBuildDetails = pilgrimBuildDetails;
                        State = ModelStateEnum.Active;
                    }));
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    State = ModelStateEnum.Invalid;
                }));
            }
        }

        #region BuildModel

        private PilgrimBuildDetail[] _pilgrimBuildDetails;

        public PilgrimBuildDetail[] PilgrimBuildDetails
        {
            get
            {
                VerifyCalledOnUiThread();
                return _pilgrimBuildDetails;
            }
            set
            {
                VerifyCalledOnUiThread();
                if (_pilgrimBuildDetails == value) return;

                _pilgrimBuildDetails = value;
                SendPropertyChanged("PilgrimBuildDetails");
            }
        }

        #endregion
    }
}
