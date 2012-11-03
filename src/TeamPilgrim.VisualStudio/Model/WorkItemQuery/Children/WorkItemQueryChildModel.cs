namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery.Children
{
    public abstract class WorkItemQueryChildModel : BaseModel
    {
        public WorkItemQueryFolderModel ParentQueryFolderModel { get; protected internal set; }

        protected IWorkItemQueryCommandModel workItemQueryCommandModel;

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