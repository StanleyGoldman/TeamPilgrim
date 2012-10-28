namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems
{
    public abstract class QueryItemNode : BaseNode
    {
        protected IQueryItemCommandModel queryItemCommandModel;

        protected QueryItemNode(IQueryItemCommandModel queryItemCommandModel)
        {
            this.queryItemCommandModel = queryItemCommandModel;
        }

        public IQueryItemCommandModel QueryItemCommandModel
        {
            get { return queryItemCommandModel; }
        }
    }
}