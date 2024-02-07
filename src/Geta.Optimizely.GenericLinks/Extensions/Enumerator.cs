// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks.Extensions;

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
