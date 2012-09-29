using System;
using System.ComponentModel;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using NUnit.Framework;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Tests.Model
{
    [TestFixture]
    public class PigrimModelTest
    {
        public class TestPilgrimServiceModelProvider : IPilgrimServiceModelProvider
        {
            public PilgrimProjectCollection[] LastPilgrimProjectCollections { get; private set; }

            public bool TryGetCollections(out PilgrimProjectCollection[] collections)
            {
                collections = new PilgrimProjectCollection[0];
                LastPilgrimProjectCollections = collections;
                return true;
            }

            public PilgrimProject[] LastPilgrimProjects { get; private set; }

            public bool TryGetProjects(out PilgrimProject[] projects, Uri tpcAddress)
            {
                throw new NotImplementedException();
            }

            public bool TryGetBuildServiceProvider(out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress)
            {
                throw new NotImplementedException();
            }

            public bool TryGetProjectsAndBuildServiceProvider(out PilgrimProject[] projects, out IPilgrimBuildServiceModelProvider buildService, Uri tpcAddress)
            {
                projects = new PilgrimProject[0];
                buildService = new TestPilgrimBuildServiceModelProvider();
                return true;
            }
        }

        public class TestPilgrimBuildServiceModelProvider : IPilgrimBuildServiceModelProvider
        {
            public bool TryGetBuildsByProjectName(out PilgrimBuildDetail[] pilgrimBuildDetails, string teamProject)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestInitialState()
        {
            var pilgrimModel = new PilgrimModel(new TestPilgrimServiceModelProvider());

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
            Assert.That(pilgrimModel.State, Is.EqualTo(ModelStateEnum.Invalid));
        }

        [Test]
        public void TestActivation()
        {
            var pilgrimModel = new PilgrimModel(new TestPilgrimServiceModelProvider());

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
