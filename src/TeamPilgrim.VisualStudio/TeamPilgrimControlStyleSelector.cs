using System.Windows;
using System.Windows.Controls;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.ProjectModels;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.QueryItemModels;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    public class TeamPilgrimControlStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }

        public Style SourceControlNodeStyle { get; set; }

        
        public Style BuildDefinitionWrapperStyle { get; set; }
        
        public Style ProjectCollectionModelStyle { get; set; }

        public Style ProjectModelStyle { get; set; }

        public Style WorkItemsNodeStyle { get; set; }
        
        public Style QueryFolderNodeStyle { get; set; }

        public Style QueryDefinitionNodeStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var projectCollectionModel = item as ProjectCollectionModel;
            if (projectCollectionModel != null && ProjectCollectionModelStyle != null)
            {
                return ProjectCollectionModelStyle;
            }

            var projectModel = item as ProjectModel;
            if (projectModel != null && ProjectModelStyle != null)
            {
                return ProjectModelStyle;
            }
            
            var workItemsNode = item as WorkItemsModel;
            if (workItemsNode != null && WorkItemsNodeStyle != null)
            {
                return WorkItemsNodeStyle;
            }
            
            var sourceControlNode = item as SourceControlModel;
            if (sourceControlNode != null && SourceControlNodeStyle != null)
            {
                return SourceControlNodeStyle;
            }

            var queryDefinitionNode = item as QueryDefinitionModel;
            if (queryDefinitionNode != null && QueryDefinitionNodeStyle != null)
            {
                return QueryDefinitionNodeStyle;
            }

            var queryFolderNode = item as QueryFolderModel;
            if (queryFolderNode != null && QueryFolderNodeStyle != null)
            {
                return QueryFolderNodeStyle;
            }

            var buildDefinitionWrapper = item as BuildDefinitionWrapper;
            if (buildDefinitionWrapper != null && BuildDefinitionWrapperStyle != null)
            {
                return BuildDefinitionWrapperStyle;
            }

            if(DefaultStyle != null)
            {
                return DefaultStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}