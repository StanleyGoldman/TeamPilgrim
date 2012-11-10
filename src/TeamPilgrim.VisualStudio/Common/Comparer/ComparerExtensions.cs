using System.Collections;
using System.Collections.Generic;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Comparer
{
    public static class ComparerExtensions
    {
        public static IComparer<T> ToGenericComparer<T>(this IComparer comparer)
        {
            return new UntypedComparerWrapper<T>(comparer);
        }

        public static IEqualityComparer ToEqualityComparer(this IComparer comparer)
        {
            return new ComparerEqualityComparer(comparer);
        }
        
        public static IEqualityComparer<T> ToEqualityComparer<T>(this IComparer<T> comparer)
        {
            return new ComparerEqualityComparer<T>(comparer);
        }
    }
}