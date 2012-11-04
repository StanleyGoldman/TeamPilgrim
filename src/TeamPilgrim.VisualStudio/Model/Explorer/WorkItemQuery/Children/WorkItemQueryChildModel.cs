using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Explorer.WorkItemQuery.Children
{
    public abstract class WorkItemQueryChildModel : BaseModel
    {
        public WorkItemQueryFolderModel ParentQueryFolder { get; protected internal set; }

        protected IWorkItemQueryCommandModel workItemQueryCommandModel;

        public abstract Guid Id { get; }

        protected WorkItemQueryChildModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, IWorkItemQueryCommandModel workItemQueryCommandModel)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            this.workItemQueryCommandModel = workItemQueryCommandModel;
        }

        public IWorkItemQueryCommandModel WorkItemQueryCommandModel
        {
            get { return workItemQueryCommandModel; }
        }
    }
}