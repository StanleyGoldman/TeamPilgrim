using FluentAssertions;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters;
using Microsoft.TeamFoundation.VersionControl.Client;
using NUnit.Framework;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Tests.Common.Converters
{
    [TestFixture]
    public class ChangeTypeDescriptionConverterTest
    {
        [Test]
        public void GetDescriptionAddLock()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Add | ChangeType.Edit | ChangeType.Encoding | ChangeType.Lock, "add");
            changeTypeDescription.Should().Be("add, lock");
        }

        [Test]
        public void GetDescriptionLockEdit()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Edit | ChangeType.Lock, "edit");
            changeTypeDescription.Should().Be("lock, edit");
        }

        [Test]
        public void GetDescriptionDeleteLock()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Delete | ChangeType.Lock, "delete");
            changeTypeDescription.Should().Be("delete, lock");
        }

        [Test]
        public void GetDescriptionLock()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Lock, "lock");
            changeTypeDescription.Should().Be("lock");
        }

        [Test]
        public void GetDescriptionMergeEdit()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Edit | ChangeType.Merge, "merge, edit");
            changeTypeDescription.Should().Be("merge, edit");
        }

        [Test]
        public void GetDescriptionMergeBranch()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Branch | ChangeType.Merge, "merge, branch");
            changeTypeDescription.Should().Be("merge, branch");
        }

        [Test]
        public void GetDescriptionMergeDelete()
        {
            var changeTypeDescription = ChangeTypeDescriptionConverter.GetChangeTypeDescription(ChangeType.Delete | ChangeType.Merge, "merge, delete");
            changeTypeDescription.Should().Be("merge, delete");
        }
    }
}
