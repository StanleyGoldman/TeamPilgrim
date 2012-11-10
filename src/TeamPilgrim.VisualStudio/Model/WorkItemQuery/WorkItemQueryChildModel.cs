using System;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery
{
    public abstract class WorkItemQueryChildModel : BaseModel
    {
        public WorkItemQueryFolderModel ParentQueryFolder { get; protected internal set; }

        public IWorkItemQueryCommandModel WorkItemQueryCommandModel { get; protected set; }
      
        public int Depth { get; protected set; }
        
        public abstract Guid Id { get; }

        protected WorkItemQueryChildModel(IWorkItemQueryCommandModel workItemQueryCommandModel, int depth)
        {
            WorkItemQueryCommandModel = workItemQueryCommandModel;
            Depth = depth;
        }
    }
}