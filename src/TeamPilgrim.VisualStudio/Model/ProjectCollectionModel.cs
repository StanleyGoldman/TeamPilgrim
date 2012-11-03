using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class ProjectCollectionModel : BaseModel
    {
        public TeamPilgrimModel TeamPilgrimModel { get; private set; }

        public TfsTeamProjectCollection TfsTeamProjectCollection { get; private set; }

        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public ProjectCollectionModel(TfsTeamProjectCollection pilgrimProjectCollection, TeamPilgrimModel teamPilgrimModel, IPilgrimServiceModelProvider pilgrimServiceModelProvider)
        {
            ProjectModels = new ObservableCollection<ProjectModel>();

            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            TfsTeamProjectCollection = pilgrimProjectCollection;
            TeamPilgrimModel = teamPilgrimModel;

            Project[] projects;
            if (_pilgrimServiceModelProvider.TryGetProjects(out projects, TfsTeamProjectCollection.Uri))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                    {
                        var pilgrimProjectModels = projects
                            .Select(project => new ProjectModel(_pilgrimServiceModelProvider, TfsTeamProjectCollection, project));

                        foreach (var pilgrimProjectModel in pilgrimProjectModels)
                        {
                            ProjectModels.Add(pilgrimProjectModel);
                        }
                    }));
            }
        }

        public ObservableCollection<ProjectModel> ProjectModels { get; private set; }
    }
}