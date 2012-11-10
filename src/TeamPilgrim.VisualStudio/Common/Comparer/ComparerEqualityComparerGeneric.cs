using System.Collections.Generic;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer
{
    public class ComparerEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly IComparer<T> _comparer;

        public ComparerEqualityComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return _comparer.Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}