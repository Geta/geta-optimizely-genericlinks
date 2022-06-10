// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer;
using EPiServer.ContentApi.Core.Serialization;
using EPiServer.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Geta.Optimizely.GenericLinks.ContentDeliveryApi
{
    public class GenericLinkCollectionConverterProvider : IPropertyConverterProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public GenericLinkCollectionConverterProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual int SortOrder => 100;

        public virtual IPropertyConverter Resolve(PropertyData propertyData)
        {
            var valueType = propertyData.PropertyValueType;
            var linkDataType = valueType.GenericTypeArguments.Length > 0 ? valueType.GenericTypeArguments[0] : null;
            if (linkDataType is null)
                return null;

            if (!typeof(LinkDataCollection).IsAssignableFrom(valueType)
                || !typeof(PropertyLinkDataCollection<>).MakeGenericType(linkDataType).IsAssignableFrom(propertyData.GetOriginalType()))
            {
                return null;
            }

            var instanceType = typeof(PropertyGenericLinkCollectionConverter<>).MakeGenericType(linkDataType);

            return ActivatorUtilities.CreateInstance(_serviceProvider, instanceType) as IPropertyConverter;
        }
    }
}
