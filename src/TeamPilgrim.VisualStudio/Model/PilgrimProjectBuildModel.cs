using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PilgrimProjectBuildModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;
        private readonly IPilgrimBuildServiceModelProvider _buildServiceModelProvider;
        private readonly TfsTeamProjectCollection _collection;
        private readonly Project _project;

        public PilgrimProjectBuildModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, IPilgrimBuildServiceModelProvider buildServiceModelProvider, TfsTeamProjectCollection collection, Project project)
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
            IBuildDetail[] pilgrimBuildDetails;
            if (_buildServiceModelProvider.TryGetBuildsByProjectName(out pilgrimBuildDetails, _project.Name))
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

        private IBuildDetail[] _pilgrimBuildDetails;

        public IBuildDetail[] PilgrimBuildDetails
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
