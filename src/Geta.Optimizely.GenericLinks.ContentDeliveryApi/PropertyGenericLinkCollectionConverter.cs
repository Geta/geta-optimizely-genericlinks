// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.ContentApi.Core.Serialization;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace Geta.Optimizely.GenericLinks.ContentDeliveryApi
{
    public class PropertyGenericLinkCollectionConverter<T> : IPropertyConverter where T : LinkData, new()
    {
        private readonly IUrlResolver _urlResolver;

        public PropertyGenericLinkCollectionConverter(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public virtual IPropertyModel Convert(PropertyData propertyData, ConverterContext contentMappingContext)
        {
            return new GenericLinkCollectionPropertyModel<T>(propertyData as PropertyLinkDataCollection<T>, _urlResolver);
        }
    }
}
