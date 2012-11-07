using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer.BuildDefinitions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer
{
    public class ProjectModel : BaseModel
    {
        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public TeamPilgrimModel TeamPilgrimModel { get; private set; }

        public BuildDefinitionsModel BuildDefinitionsModel { get; private set; }
        
        public WorkItemQueryContainerModel WorkItemQueryContainerModel { get; private set; }

        public ObservableCollection<BaseModel> ChildObjects { get; private set; }

        public ProjectModel(ITeamPilgrimServiceModelProvider teamPilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TeamPilgrimModel teamPilgrimModel, TfsTeamProjectCollection projectCollection, Project project)
            :base(teamPilgrimServiceModelProvider, teamPilgrimVsService)
        {
            TeamPilgrimModel = teamPilgrimModel;
            ProjectCollection = projectCollection;
            Project = project;
            
            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);

            BuildDefinitionsModel = new BuildDefinitionsModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, projectCollection, project);

            WorkItemQueryContainerModel = new WorkItemQueryContainerModel(teamPilgrimServiceModelProvider, teamPilgrimVsService, teamPilgrimModel, projectCollection, project);

            ChildObjects = new ObservableCollection<BaseModel>
                {
                    WorkItemQueryContainerModel, 
                    new ReportsModel(teamPilgrimServiceModelProvider, teamPilgrimVsService),
                    BuildDefinitionsModel,
                    new TeamMembersModel(teamPilgrimServiceModelProvider, teamPilgrimVsService),
                    new SourceControlModel(teamPilgrimServiceModelProvider, teamPilgrimVsService)
                };
        }

        #region OpenSourceControl

        public RelayCommand OpenSourceControlCommand { get; private set; }

        private void OpenSourceControl()
        {
            teamPilgrimVsService.OpenSourceControl(Project.Name);
        }

        private bool CanOpenSourceControl()
        {
            return true;
        }

        #endregion
    }
}
