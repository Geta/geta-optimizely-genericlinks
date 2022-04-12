using System.Collections;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks.Extensions
{
    internal static class Enumerator
    {
        public static IEnumerator<T> Empty<T>()
        {
            yield break;
        }

        public static IEnumerator Empty()
        {
            yield break;
        }
    }
}
