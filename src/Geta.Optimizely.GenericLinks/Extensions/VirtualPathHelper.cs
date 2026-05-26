// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace Geta.Optimizely.GenericLinks.Extensions;

internal static class VirtualPathHelper
{
    internal static string ToAbsoluteOrSame(string? path)
    {
        if (path is not null && path.StartsWith("~/", StringComparison.Ordinal))
            return path[1..];

        return path ?? string.Empty;
    }
}
