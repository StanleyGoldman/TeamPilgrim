using System;
using System.ComponentModel;
using System.Windows.Threading;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using NUnit.Framework;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Tests.Model
{
    [TestFixture]
    public class PigrimModelTest
    {
        public class TestPilgrimModelProvider : IPilgrimModelProvider
        {
            public PilgrimProjectCollection[] LastPilgrimProjectCollections { get; private set; }

            public bool TryGetCollections(out PilgrimProjectCollection[] collections)
            {
                collections = new PilgrimProjectCollection[] { };
                LastPilgrimProjectCollections = collections;
                return true;
            }

            public PilgrimProject[] LastPilgrimProjects { get; private set; }

            public bool TryGetProjects(out PilgrimProject[] projects, Uri tpcAddress)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestInitialState()
        {
            var pilgrimModel = new PilgrimModel(new TestPilgrimModelProvider());

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
            Assert.That(pilgrimModel.State, Is.EqualTo(ModelStateEnum.Invalid));
        }

        [Test]
        public void TestActivation()
        {
            var pilgrimModel = new PilgrimModel(new TestPilgrimModelProvider());

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
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
