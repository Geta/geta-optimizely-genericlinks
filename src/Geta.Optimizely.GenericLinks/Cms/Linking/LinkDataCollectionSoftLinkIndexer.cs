// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;

namespace Geta.Optimizely.GenericLinks.Cms.Linking;

internal class LinkDataCollectionSoftLinkIndexer : IPropertySoftLinkIndexer<LinkDataCollection>, IPropertySoftLinkIndexer
{
    private readonly LinkDataToSoftLinkResolver _softLinkResolver;

    public LinkDataCollectionSoftLinkIndexer(LinkDataToSoftLinkResolver softLinkResolver)
    {
        _softLinkResolver = softLinkResolver;
    }

    public IEnumerable<SoftLink> ResolveReferences(LinkDataCollection property, IContent owner)
    {
        if (property == null)
            return [];

        var softLinkSet = new HashSet<SoftLink>();

        foreach (var linkData in property.GetLinks())
        {
            var softLinks = _softLinkResolver.Resolve(linkData, owner);

            foreach (var softLink in softLinks)
            {
                softLinkSet.Add(softLink);
            }
        }

        return softLinkSet;
    }
}
