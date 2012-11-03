using System;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public abstract class WorkItemQueryChildModel : BaseModel
    {
        public WorkItemQueryFolderModel ParentQueryFolder { get; protected internal set; }

        protected IWorkItemQueryCommandModel workItemQueryCommandModel;

        public abstract Guid Id { get; }

        protected WorkItemQueryChildModel(IWorkItemQueryCommandModel workItemQueryCommandModel)
        {
            this.workItemQueryCommandModel = workItemQueryCommandModel;
        }

        public IWorkItemQueryCommandModel WorkItemQueryCommandModel
        {
            get { return workItemQueryCommandModel; }
        }
    }
}