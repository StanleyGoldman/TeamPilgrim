using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces.VisualStudio;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer
{
    public class ProjectCollectionServiceModel : BaseServiceModel
    {
        private readonly TeamPilgrimServiceModel _teamPilgrimServiceModel;

        public ObservableCollection<ProjectServiceModel> ProjectModels { get; private set; }

        public TfsTeamProjectCollection TfsTeamProjectCollection { get; private set; }

        public ProjectCollectionServiceModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimServiceModel teamPilgrimServiceModel, TfsTeamProjectCollection pilgrimProjectCollection)
            : base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectModels = new ObservableCollection<ProjectServiceModel>();

            TfsTeamProjectCollection = pilgrimProjectCollection;
            _teamPilgrimServiceModel = teamPilgrimServiceModel;

            Populate();
        }

        private void Populate()
        {
            Project[] projects;
            if (teamPilgrimServiceModelProvider.TryGetProjects(out projects, TfsTeamProjectCollection.Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        ProjectModels.Clear();

                        var pilgrimProjectModels = projects
                            .Select(
                                project =>
                                new ProjectServiceModel(teamPilgrimServiceModelProvider, teamPilgrimVsService,
                                                        _teamPilgrimServiceModel, TfsTeamProjectCollection, project));

                        foreach (var pilgrimProjectModel in pilgrimProjectModels)
                        {
                            ProjectModels.Add(pilgrimProjectModel);
                        }
                    }));
            }
        }

        #region Refresh Command

        protected override void Refresh()
        {
            Populate();
        }

        protected override bool CanRefresh()
        {
            return true;
        }

        #endregion
    }
}