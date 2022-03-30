using System;

namespace Geta.GenericLinks.Extensions
{
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
}
