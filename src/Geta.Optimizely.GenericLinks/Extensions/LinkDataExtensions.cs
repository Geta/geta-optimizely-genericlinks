// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Html;

namespace Geta.Optimizely.GenericLinks.Extensions;

public static class LinkDataExtensions
{
    public static string GetMappedHref(this ILinkData linkData)
    {
        return VirtualPathHelper.ToAbsoluteOrSame(linkData.Href);
    }

    public static string? ToMappedLink(this ILinkData linkData, ILinkHtmlSerializer? linkHtmlSerializer = null)
    {
        linkHtmlSerializer ??= ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>();

        var hrefValue = GetMappedHref(linkData);

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
