using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer
{
    public class ProjectCollectionModel : BaseModel
    {
        private readonly TeamPilgrimModel _teamPilgrimModel;

        public ObservableCollection<ProjectModel> ProjectModels { get; private set; }

        public TfsTeamProjectCollection TfsTeamProjectCollection { get; private set; }

        public ProjectCollectionModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimModel teamPilgrimModel, TfsTeamProjectCollection pilgrimProjectCollection)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectModels = new ObservableCollection<ProjectModel>();

            TfsTeamProjectCollection = pilgrimProjectCollection;
            _teamPilgrimModel = teamPilgrimModel;

            Project[] projects;
            if (base.pilgrimServiceModelProvider.TryGetProjects(out projects, TfsTeamProjectCollection.Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        var pilgrimProjectModels = projects
                            .Select(project => new ProjectModel(base.pilgrimServiceModelProvider, teamPilgrimVsService, TfsTeamProjectCollection, project));

                        foreach (var pilgrimProjectModel in pilgrimProjectModels)
                        {
                            ProjectModels.Add(pilgrimProjectModel);
                        }
                    }));
            }
        }
    }
}