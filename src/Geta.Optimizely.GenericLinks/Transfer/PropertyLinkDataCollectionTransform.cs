// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core.Transfer;
using EPiServer.Core;
using EPiServer;
using EPiServer.Core.Transfer.Internal;
using System.Linq;
using Geta.Optimizely.GenericLinks.Html;

namespace Geta.Optimizely.GenericLinks.Transfer;

public sealed class PropertyLinkDataCollectionTransform<TLinkData> : PropertyTransform<PropertyLinkDataCollection<TLinkData>>
    where TLinkData : ILinkData, new()
{
    private readonly IContentLoader _contentLoader;
    private readonly ILinkHtmlSerializer _linkHtmlSerializer;
    private readonly IImplicitContentExporter _implicitContentExporter;

    public PropertyLinkDataCollectionTransform(
        IImplicitContentExporter implicitContentExporter,
        ILinkHtmlSerializer linkHtmlSerializer,
        IContentLoader contentLoader)
    {
        _implicitContentExporter = implicitContentExporter;
        _linkHtmlSerializer = linkHtmlSerializer;
        _contentLoader = contentLoader;
    }

    protected override bool TransformForExport(
      PropertyLinkDataCollection<TLinkData> source,
      RawProperty output,
      PropertyExportContext context)
    {
        if (source.Value is null)
        {
            output.Value = null;
            return true;
        }

        var sourceLinks = source.Links!.Select(l => l.ReferencedPermanentLinkIds);

        foreach (var guids in sourceLinks)
        {
            foreach (var contentGuid in guids)
            {
                if (_contentLoader.TryGet<IContent>(contentGuid, out var content))
                    _implicitContentExporter.ExportDependentContent(content, context.TransferContext);
            }
        }

        output.Value = _linkHtmlSerializer.Serialize(source.Links, StringMode.InternalMode);
        return true;
    }
}
