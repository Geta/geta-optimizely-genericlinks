// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.ContentApi.Core.Serialization;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace Geta.Optimizely.GenericLinks.ContentDeliveryApi
{
    public class PropertyGenericLinkConverter<T> : IPropertyConverter where T : LinkData, new()
    {
        private readonly IUrlResolver _urlResolver;

        public PropertyGenericLinkConverter(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public virtual IPropertyModel Convert(PropertyData propertyData, ConverterContext contentMappingContext)
        {
            return new GenericLinkPropertyModel<T>(propertyData as PropertyLinkData<T>, _urlResolver);
        }
    }
}
