// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace Geta.Optimizely.GenericLinks.Extensions;

internal static class StringExtensions
{
    public static string ToCamel(this string input)
    {
        if (input.Length == 0)
            return input;

        var firstChar = input[0];

        if (!char.IsUpper(firstChar))
            return input;

        var buffer = input.ToCharArray();

        buffer[0] = char.ToLower(firstChar);

        return new string(buffer);
    }
}
