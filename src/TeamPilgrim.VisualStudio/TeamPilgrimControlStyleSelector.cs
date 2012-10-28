using System.Windows;
using System.Windows.Controls;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.Project;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.Nodes.QueryItems;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    public class TeamPilgrimControlStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }

        public Style SourceControlNodeStyle { get; set; }

        public Style QueryDefinitionNodeStyle { get; set; }
        
        public Style BuildDefinitionWrapperStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var sourceControlNode = item as SourceControlNode;
            if (sourceControlNode != null && SourceControlNodeStyle != null)
            {
                return SourceControlNodeStyle;
            }

            var queryDefinitionNode = item as QueryDefinitionNode;
            if (queryDefinitionNode != null && QueryDefinitionNodeStyle != null)
            {
                return QueryDefinitionNodeStyle;
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