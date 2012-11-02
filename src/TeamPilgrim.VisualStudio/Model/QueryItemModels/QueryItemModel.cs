namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels
{
    public abstract class QueryItemModel : BaseModel
    {
        protected IQueryItemCommandModel queryItemCommandModel;

        protected QueryItemModel(IQueryItemCommandModel queryItemCommandModel)
        {
            this.queryItemCommandModel = queryItemCommandModel;
        }

        public IQueryItemCommandModel QueryItemCommandModel
        {
            get { return queryItemCommandModel; }
        }
    }
}