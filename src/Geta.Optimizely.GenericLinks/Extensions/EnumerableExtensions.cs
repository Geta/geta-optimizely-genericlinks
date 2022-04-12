using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static TType? FirstOfType<TType>(this IEnumerable list)
        {
            return list.OfType<TType>().FirstOrDefault();
        }
    }
}
