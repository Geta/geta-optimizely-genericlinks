// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Data.Entity;
using System;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks;

public interface ILinkData : ICloneable, IReferenceMap, IModifiedTrackable, IReadOnly<ILinkData>
{
    IDictionary<string, string> Attributes { get; }

    string? Text { get; set; }

    string? Href { get; set; }

    string? Target { get; set; }

    string? Title { get; set; }

    void SetAttributes(IDictionary<string, string> attributes);
    IDictionary<string, string> GetAttributes();
    IEnumerable<ContentReference> GetReferencedContent();
}
