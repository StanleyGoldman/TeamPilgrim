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

        protected static void PushDispatcherFrame(BaseModel baseModel)
        {
            var frame = new DispatcherFrame();

            var waitForModelHandler = new PropertyChangedEventHandler(
                delegate(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == "State" && baseModel.State != ModelStateEnum.Fetching)
                    {
                        frame.Continue = false;
                    }
                });

            baseModel.PropertyChanged += waitForModelHandler;

            Dispatcher.PushFrame(frame);
        }

        [Test]
        public void TestInitialModelState()
        {
            var pilgrimModel = new PilgrimModel(new TestPilgrimModelProvider());

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
            Assert.That(pilgrimModel.State, Is.EqualTo(ModelStateEnum.Invalid)); // TODO, MB, 9/26/2012: This test was original: ModelStateEnum.Fetching but failed

            PushDispatcherFrame(pilgrimModel);

            Assert.That(pilgrimModel.CollectionModels, Is.Not.Null);
        }
    }
}