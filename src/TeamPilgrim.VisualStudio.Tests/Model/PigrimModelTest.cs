using System;
using System.ComponentModel;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NUnit.Framework;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Tests.Model
{
    [TestFixture]
    public class PigrimModelTest
    {
        public class TestPilgrimServiceModelProvider : IPilgrimServiceModelProvider
        {
            public TfsTeamProjectCollection[] LastPilgrimProjectCollections { get; private set; }

            public bool TryGetCollections(out TfsTeamProjectCollection[] collections)
            {
                collections = new TfsTeamProjectCollection[0];
                LastPilgrimProjectCollections = collections;
                return true;
            }

            public bool TryGetCollection(out TfsTeamProjectCollection collection, Uri tpcAddress)
            {
                throw new NotImplementedException();
            }

            public Project[] LastPilgrimProjects { get; private set; }

            public bool TryGetProjects(out Project[] projects, Uri tpcAddress)
            {
                throw new NotImplementedException();
            }

            public bool TryGetBuildServiceProvider(out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress)
            {
                throw new NotImplementedException();
            }

            public bool TryGetProjectsAndBuildServiceProvider(out Project[] projects, out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress)
            {
                projects = new Project[0];
                buildService = new TestPilgrimBuildServiceModelProvider();
                return true;
            }

            public bool TryDeleteQueryDefinition(out bool result, TfsTeamProjectCollection teamProjectCollection, Project teamProject, Guid queryId)
            {
                throw new NotImplementedException();
            }
        }

        public class TestPilgrimBuildServiceModelProvider : IPilgrimBuildServiceModelProvider
        {
            public bool TryGetBuildDefinitionsByProjectName(out BuildDefinitionWrapper[] buildDefinitions, string teamProject)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestInitialState()
        {
            var pilgrimModel = new TeamPilgrimModel(new TestPilgrimServiceModelProvider());

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
            Assert.That(pilgrimModel.State, Is.EqualTo(ModelStateEnum.Invalid));
        }

        [Test]
        public void TestActivation()
        {
            var pilgrimModel = new TeamPilgrimModel(new TestPilgrimServiceModelProvider());

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
            Assert.That(pilgrimModel.CollectionModels, Is.Empty);
            Assert.That(pilgrimModel.State, Is.EqualTo(ModelStateEnum.Invalid));

            pilgrimModel.Activate();

            var frame = new DispatcherFrame();

            var waitForModelHandler = new PropertyChangedEventHandler(
                delegate(object sender, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == "State" && pilgrimModel.State != ModelStateEnum.Invalid)
                        {
                            frame.Continue = false;
                        }
                    });

            pilgrimModel.PropertyChanged += waitForModelHandler;

            Dispatcher.PushFrame(frame);

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
        }
    }
}
