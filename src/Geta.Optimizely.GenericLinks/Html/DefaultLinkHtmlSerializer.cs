// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Net;
using System.Text;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using Geta.Optimizely.GenericLinks.Extensions;

namespace Geta.Optimizely.GenericLinks.Html
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

        public virtual string Serialize<TLinkData>(TLinkData? link, StringMode mode) where TLinkData : ILinkData
        {
            if (link is null)
                return string.Empty;

            return SerializeLinks(link.Yield(), mode);
        }

        public virtual string Serialize<TLinkData>(LinkDataCollection<TLinkData>? links, StringMode mode)
            where TLinkData : ILinkData
        {
            if (links is null)
                return string.Empty;

            return SerializeLinks(links, mode);
        }

        protected virtual string SerializeLinks<TLinkData>(IEnumerable<TLinkData> links, StringMode mode)
            where TLinkData : ILinkData
        {
            if (!links.Any())
                return string.Empty;

            if (StringMode.EditMode == mode)
                return string.Empty;

            var stringBuilder = new StringBuilder(256);

            stringBuilder.Append("<ul>");

            foreach (ILinkData link in links)
            {
                WriteLink(mode, stringBuilder, link);
            }

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }

        protected virtual void WriteLink(StringMode mode, StringBuilder stringBuilder, ILinkData link)
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
