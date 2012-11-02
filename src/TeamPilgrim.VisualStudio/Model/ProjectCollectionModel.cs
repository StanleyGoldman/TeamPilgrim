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
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            TfsTeamProjectCollection = pilgrimProjectCollection;
            TeamPilgrimModel = teamPilgrimModel;
        
            Project[] projects;
            if (_pilgrimServiceModelProvider.TryGetProjects(out projects, TfsTeamProjectCollection.Uri))
            {
                var pilgrimProjectModels = projects
                    .Select(project => new ProjectModel(_pilgrimServiceModelProvider, TfsTeamProjectCollection, project, new ProjectBuildModel(_pilgrimServiceModelProvider, TfsTeamProjectCollection, project)))
                    .ToArray();

                ProjectModels = pilgrimProjectModels;
            }
        }

        #region ProjectModels

        private ProjectModel[] _projectModels = new ProjectModel[0];

        public ProjectModel[] ProjectModels
        {
            get
            {
                return _projectModels;
            }
            private set
            {
                if (_projectModels == value) return;

                _projectModels = value;
                SendPropertyChanged("ProjectModels");
            }
        }

        #endregion
    }
}