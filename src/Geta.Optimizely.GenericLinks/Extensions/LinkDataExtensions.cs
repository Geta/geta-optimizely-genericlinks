// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Html;

namespace Geta.Optimizely.GenericLinks.Extensions
{
    public static class LinkDataExtensions
    {
        public static string GetMappedHref(this ILinkData linkData, IVirtualPathResolver? virtualPathResolver = null)
        {
            virtualPathResolver ??= ServiceLocator.Current.GetInstance<IVirtualPathResolver>();
            return virtualPathResolver.ToAbsoluteOrSame(linkData.Href);
        }

        public static string? ToMappedLink(this ILinkData linkData, IVirtualPathResolver? virtualPathResolver = null, ILinkHtmlSerializer? linkHtmlSerializer = null)
        {
            virtualPathResolver ??= ServiceLocator.Current.GetInstance<IVirtualPathResolver>();
            linkHtmlSerializer ??= ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>();
            
            var hrefValue = GetMappedHref(linkData, virtualPathResolver);

            return linkHtmlSerializer.CreateLink(hrefValue, linkData);
        }

        public static string? ToPermanentLink(this ILinkData linkData, IUrlResolver? urlResolver = null, ILinkHtmlSerializer? linkHtmlSerializer = null)
        {
            linkHtmlSerializer ??= ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>();
            urlResolver ??= ServiceLocator.Current.GetInstance<IUrlResolver>();

            var hrefValue = urlResolver.GetPermanent(linkData.Href, true);

            return linkHtmlSerializer.CreateLink(hrefValue, linkData);
        }
    }
}
