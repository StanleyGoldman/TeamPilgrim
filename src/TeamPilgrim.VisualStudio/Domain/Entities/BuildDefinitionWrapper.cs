using System;
using Microsoft.TeamFoundation.Build.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities
{
    public class BuildDefinitionWrapper
    {
        private readonly IBuildDefinition _buildDefinition;

        public BuildDefinitionWrapper(IBuildDefinition buildDefinition)
        {
            _buildDefinition = buildDefinition;
        }

        public string Name
        {
            get { return _buildDefinition.Name; }
        }

        public string Id
        {
            get { return _buildDefinition.Id; }
        }

        public Uri Uri
        {
            get { return _buildDefinition.Uri; }
        }
    }
}