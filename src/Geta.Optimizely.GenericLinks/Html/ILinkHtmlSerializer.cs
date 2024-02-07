// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace Geta.Optimizely.GenericLinks.Html;

public interface ILinkHtmlSerializer
{
    string Serialize<TLinkData>(TLinkData? link, StringMode mode)
        where TLinkData : ILinkData;

    string Serialize<TLinkData>(LinkDataCollection<TLinkData>? links, StringMode mode)
        where TLinkData : ILinkData;

    string? CreateLink(string? hrefValue, ILinkData linkData);
}
