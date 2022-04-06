using System.Collections.Generic;

namespace Geta.GenericLinks.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
