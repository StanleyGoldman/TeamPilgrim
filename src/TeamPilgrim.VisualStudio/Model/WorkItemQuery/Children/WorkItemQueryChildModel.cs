using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.BusinessInterfaces;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public abstract class WorkItemQueryChildModel : BaseModel
    {
        public WorkItemQueryFolderModel ParentQueryFolder { get; protected internal set; }

        public IWorkItemQueryCommandModel WorkItemQueryCommandModel { get; protected set; }
      
        public int Depth { get; protected set; }
        
        public abstract Guid Id { get; }

        protected WorkItemQueryChildModel(IPilgrimServiceModelProvider pilgrimServiceModelProvider, ITeamPilgrimVsService teamPilgrimVsService, IWorkItemQueryCommandModel workItemQueryCommandModel, int depth)
            : base(pilgrimServiceModelProvider, teamPilgrimVsService)
        {
            WorkItemQueryCommandModel = workItemQueryCommandModel;
            Depth = depth;
        }
    }
}