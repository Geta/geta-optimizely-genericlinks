// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace Geta.Optimizely.GenericLinks.Tests.Services;

public class FakeUrlResolver : IUrlResolver
{
    public string GetUrl(ContentReference contentLink, string language, UrlResolverArguments urlResolverArguments)
    {
        return $"http://localhost/{contentLink}";
    }

    public string GetUrl(UrlBuilder urlBuilderWithInternalUrl, UrlResolverArguments arguments)
    {
        return urlBuilderWithInternalUrl.ToString();
    }

    public ContentRouteData Route(UrlBuilder urlBuilder, RouteArguments routeArguments)
    {
        return new ContentRouteData(null, "en-US", string.Empty, null, null);
    }

    public bool TryToPermanent(string url, out string permanentUrl)
    {
        permanentUrl = string.Empty;
        return false;
    }
}
