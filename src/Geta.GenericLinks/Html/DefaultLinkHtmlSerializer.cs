using System.Net;
using System.Text;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Geta.GenericLinks.Html
{
    public class DefaultLinkHtmlSerializer : ILinkHtmlSerializer
    {
        private readonly IVirtualPathResolver _virtualPathResolver;
        private readonly IUrlResolver _urlResolver;

        public DefaultLinkHtmlSerializer(IVirtualPathResolver virtualPathResolver, IUrlResolver urlResolver)
        {
            _virtualPathResolver = virtualPathResolver;
            _urlResolver = urlResolver;
        }

        public string Serialize<TLinkData>(LinkDataCollection<TLinkData>? links, StringMode mode)
            where TLinkData : ILinkData
        {
            if (links is null)
                return string.Empty;

            if (links.Count == 0)
                return string.Empty;

            if (StringMode.EditMode == mode)
                return string.Empty;

            var stringBuilder = new StringBuilder(256);

            stringBuilder.Append("<ul>");

            foreach (ILinkData link in links)
            {
                stringBuilder.Append("<li>");

                string href;

                if (mode == StringMode.ViewMode)
                {
                    href = _virtualPathResolver.ToAbsoluteOrSame(link.Href);
                }
                else
                {
                    href = _urlResolver.GetPermanent(link.Href, enableFallback: true);
                }

                stringBuilder.Append(CreateLink(href, link));
                stringBuilder.Append("</li>");
            }

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }

        protected virtual string? CreateLink(string hrefValue, ILinkData linkData)
        {
            if (string.IsNullOrEmpty(hrefValue))
            {
                return WebUtility.HtmlEncode(linkData.Text);
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<a");
            foreach (var attribute in linkData.Attributes)
            {
                if (string.IsNullOrEmpty(attribute.Value))
                    continue;

                var value = attribute.Value;

                if (attribute.Key == "href")
                    value = hrefValue;

                stringBuilder.Append($" {attribute.Key}=\"{WebUtility.HtmlEncode(value)}\"");
            }

            stringBuilder.Append($">{WebUtility.HtmlEncode(linkData.Text)}</a>");
            return stringBuilder.ToString();
        }
    }
}
