// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core.Transfer;
using EPiServer;
using EPiServer.Core.Transfer.Internal;
using EPiServer.Core;
using Geta.Optimizely.GenericLinks.Html;

namespace Geta.Optimizely.GenericLinks.Transfer
{
    public sealed class PropertyLinkDataTransform<TLinkData> : PropertyTransform<PropertyLinkData<TLinkData>>
        where TLinkData : LinkData, new()
    {
        private readonly IContentLoader _contentLoader;
        private readonly IImplicitContentExporter _implicitContentExporter;
        private readonly ILinkHtmlSerializer _linkHtmlSerializer;

        public PropertyLinkDataTransform(
            IImplicitContentExporter implicitContentExporter,
            ILinkHtmlSerializer linkHtmlSerializer,
            IContentLoader contentLoader)
        {
            _implicitContentExporter = implicitContentExporter;
            _linkHtmlSerializer = linkHtmlSerializer;
            _contentLoader = contentLoader;
        }

        protected override bool TransformForExport(PropertyLinkData<TLinkData> source, RawProperty output, PropertyExportContext context)
        {
            if (source.Value is null)
            {
                output.Value = null;
                return true;
            }

            foreach (var referencedPermanentLinkId in source.Link!.ReferencedPermanentLinkIds)
            {
                if (_contentLoader.TryGet(referencedPermanentLinkId, out IContent content))
                    _implicitContentExporter.ExportDependentContent(content, context.TransferContext);
            }

            output.Value = _linkHtmlSerializer.Serialize(source.Link, StringMode.InternalMode);
            return true;
        }
    }
}
