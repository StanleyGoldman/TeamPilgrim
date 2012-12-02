using System;
using System.Linq;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges
{
    public class PreviouslySelectedWorkItemQuery : IEquatable<PreviouslySelectedWorkItemQuery>
    {
        private readonly string _workItemQueryPath;

        public string WorkItemQueryPath
        {
            get { return _workItemQueryPath; }
        }

        public string Formatted
        {
            get
            {
                var strings = WorkItemQueryPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                var folderPath = strings.Take(strings.Length - 1).ToArray();

                return folderPath.Length > 0
                           ? string.Format("{0} ({1})", strings.Last(), string.Join("/", folderPath))
                           : strings[0];
            }
        }

        public PreviouslySelectedWorkItemQuery(string workItemQueryPath)
        {
            _workItemQueryPath = workItemQueryPath;
        }

        public bool Equals(PreviouslySelectedWorkItemQuery other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return this.WorkItemQueryPath == other.WorkItemQueryPath;
        }

        public sealed override bool Equals(object obj)
        {
            var otherMyItem = obj as PreviouslySelectedWorkItemQuery;
            if (ReferenceEquals(otherMyItem, null)) return false;
            return otherMyItem.Equals(this);
        }

        public override int GetHashCode()
        {
            return _workItemQueryPath.GetHashCode();
        }

        public static bool operator ==(PreviouslySelectedWorkItemQuery previouslySelectedWorkItemQuery1, PreviouslySelectedWorkItemQuery previouslySelectedWorkItemQuery2)
        {
            return Equals(previouslySelectedWorkItemQuery1, previouslySelectedWorkItemQuery2);
        }

        public static bool operator !=(PreviouslySelectedWorkItemQuery previouslySelectedWorkItemQuery1, PreviouslySelectedWorkItemQuery previouslySelectedWorkItemQuery2)
        {
            return !(previouslySelectedWorkItemQuery1 == previouslySelectedWorkItemQuery2);
        }
    }
}
