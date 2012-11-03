using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.BuildDefinitions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class ProjectModel : BaseModel
    {
        private readonly IPilgrimServiceModelProvider _pilgrimServiceModelProvider;

        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public TeamPilgrimModel TeamPilgrimModel { get; private set; }

        public BuildDefinitionsModel BuildDefinitionsModel { get; private set; }

        public ObservableCollection<BaseModel> ChildObjects { get; private set; }

        public ProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, TfsTeamProjectCollection projectCollection, Project project)
        {
            _pilgrimServiceModelProvider = pilgrimServiceModelProvider;
            ProjectCollection = projectCollection;
            Project = project;

            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);

            BuildDefinitionsModel = new BuildDefinitionsModel(_pilgrimServiceModelProvider, projectCollection, project);

            ChildObjects = new ObservableCollection<BaseModel>
                {
                    new WorkItemQueryContainerModel(_pilgrimServiceModelProvider, projectCollection, project), 
                    new ReportsModel(),
                    BuildDefinitionsModel,
                    new TeamMembersModel(),
                    new SourceControlModel()
                };
        }

        #region OpenSourceControl

        public RelayCommand OpenSourceControlCommand { get; private set; }

        private void OpenSourceControl()
        {
            TeamPilgrimPackage.TeamPilgrimVsService.OpenSourceControl(Project.Name);
        }

        private bool CanOpenSourceControl()
        {
            return true;
        }

        #endregion
    }
}
