// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Text.RegularExpressions;

namespace Geta.Optimizely.GenericLinks.Html
{
    public class DefaultAttributeSanitizer : IAttributeSanitizer
    {
        private static readonly Regex ControlCharacterFilter = new("[\\u0000-\\u001F]", RegexOptions.Compiled);

        public string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return ControlCharacterFilter.Replace(input, string.Empty);
        }
    }
}
