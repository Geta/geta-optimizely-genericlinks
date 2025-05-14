// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.Internal;

namespace Geta.Optimizely.GenericLinks.Cms.Linking;

internal class LinkDataToSoftLinkResolver
{
    private readonly SoftLinkFactory _softLinkFactory;

    public LinkDataToSoftLinkResolver(SoftLinkFactory softLinkFactory)
    {
        _softLinkFactory = softLinkFactory;
    }

    public IEnumerable<SoftLink> Resolve(ILinkData linkData, IContent owner)
    {
        var softLink = _softLinkFactory.Create(owner);
        var links = new List<SoftLink>();

        softLink.Url = linkData.Href;

        if (softLink.ReferencedContentLink == owner.ContentLink.ToReferenceWithoutVersion())
            return links;

        softLink.SoftLinkType = !ContentReference.IsNullOrEmpty(softLink.ReferencedContentLink)
            ? ReferenceType.PageLinkReference : ReferenceType.ExternalReference;

        if (softLink != null)
            links.Add(softLink);

        foreach (var reference in linkData.GetReferencedContent())
        {
            if (ContentReference.IsNullOrEmpty(reference))
                continue;

            softLink = _softLinkFactory.Create(owner);
            softLink.ReferencedContentLink = reference;

            links.Add(softLink);
        }

        return links;
    }
}
