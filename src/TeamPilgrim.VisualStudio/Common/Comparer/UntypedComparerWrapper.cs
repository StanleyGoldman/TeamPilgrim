using System.Collections;
using System.Collections.Generic;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer
{
    public class UntypedComparerWrapper<T> : IComparer<T>
    {
        private readonly IComparer _comparer;

        public UntypedComparerWrapper(IComparer comparer)
        {
            this._comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer.Compare(x, y);
        }
    }
}
