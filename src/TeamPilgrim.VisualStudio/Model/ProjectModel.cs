using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.BuildDefinitions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class ProjectModel : BaseModel
    {
        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        public Project Project { get; private set; }

        public TeamPilgrimModel TeamPilgrimModel { get; private set; }

        public BuildDefinitionsModel BuildDefinitionsModel { get; private set; }

        public ObservableCollection<BaseModel> ChildObjects { get; private set; }

        public ProjectModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, TfsTeamProjectCollection projectCollection, Project project)
            :base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            ProjectCollection = projectCollection;
            Project = project;
            
            OpenSourceControlCommand = new RelayCommand(OpenSourceControl, CanOpenSourceControl);

            BuildDefinitionsModel = new BuildDefinitionsModel(pilgrimServiceModelProvider, teamPilgrimVsService, projectCollection, project);

            ChildObjects = new ObservableCollection<BaseModel>
                {
                    new WorkItemQueryContainerModel(pilgrimServiceModelProvider, teamPilgrimVsService, projectCollection, project), 
                    new ReportsModel(pilgrimServiceModelProvider, teamPilgrimVsService),
                    BuildDefinitionsModel,
                    new TeamMembersModel(pilgrimServiceModelProvider, teamPilgrimVsService),
                    new SourceControlModel(pilgrimServiceModelProvider, teamPilgrimVsService)
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
