using System.Collections;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer
{
    public class ComparerEqualityComparer : IEqualityComparer
    {
        private readonly IComparer _comparer;

        public ComparerEqualityComparer(IComparer comparer)
        {
            _comparer = comparer;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            return _comparer.Compare(x, y) == 0;
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
