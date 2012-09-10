using System.ComponentModel;
using System.Windows.Threading;
using FluentAssertions;
using JustAProgrammer.TeamPilgrim.Domain.Entities;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;
using JustAProgrammer.TeamPilgrim.VisualStudio.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Test.Model
{
    [TestClass]
    public class PigrimModelTest
    {
        public class TestPilgrimModelProvider : IPilgrimModelProvider
        {
            public PilgrimProjectCollection[] LastPilgrimProjectCollection { get; private set; }

            public bool TryGetCollections(out PilgrimProjectCollection[] collections)
            {
                collections = new PilgrimProjectCollection[] { };
                LastPilgrimProjectCollection = collections;
                return true;
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

        [TestMethod]
        public void TestInitialModelState()
        {
            var pilgrimModel = new PilgrimModel(new TestPilgrimModelProvider());

            pilgrimModel.Collections.Should().BeNull();
            pilgrimModel.State.Should().Be(ModelStateEnum.Fetching);

            PushDispatcherFrame(pilgrimModel);

            pilgrimModel.Collections.Should().NotBeNull();
        }
    }
}
