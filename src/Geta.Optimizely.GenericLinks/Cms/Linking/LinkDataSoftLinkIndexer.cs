// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;

namespace Geta.Optimizely.GenericLinks.Cms.Linking;

internal class LinkDataSoftLinkIndexer : IPropertySoftLinkIndexer<LinkData>, IPropertySoftLinkIndexer
{
    private readonly LinkDataToSoftLinkResolver _softLinkResolver;

    public LinkDataSoftLinkIndexer(LinkDataToSoftLinkResolver softLinkResolver)
    {
        _softLinkResolver = softLinkResolver;
    }

    public IEnumerable<SoftLink> ResolveReferences(LinkData linkData, IContent owner)
    {
        if (linkData == null)
            return [];

        return _softLinkResolver.Resolve(linkData, owner);
    }
}
