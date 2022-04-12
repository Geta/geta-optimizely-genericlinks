using System.Text.RegularExpressions;

namespace Geta.Optimizely.GenericLinks.Html
{
    public class DefaultAttributeSanitizer : IAttributeSanitizer
    {
        private static readonly Regex _characterFilter = new("[\\u0000-\\u001F]", RegexOptions.Compiled);

        public string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return _characterFilter.Replace(input, string.Empty);
        }
    }
}
