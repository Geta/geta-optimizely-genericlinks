// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer;
using EPiServer.ContentApi.Core.Serialization;
using EPiServer.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Geta.Optimizely.GenericLinks.ContentDeliveryApi
{
    public class GenericLinkConverterProvider : IPropertyConverterProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public GenericLinkConverterProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual int SortOrder => 100;

        public virtual IPropertyConverter Resolve(PropertyData propertyData)
        {
            var valueType = propertyData.PropertyValueType;

            if (!typeof(LinkData).IsAssignableFrom(valueType)
              || !typeof(PropertyLinkData<>).MakeGenericType(valueType).IsAssignableFrom(propertyData.GetOriginalType()))
            {
                return null;
            }

            var instanceType = typeof(PropertyGenericLinkConverter<>).MakeGenericType(valueType);

            return ActivatorUtilities.CreateInstance(_serviceProvider, instanceType) as IPropertyConverter;
        }
    }
}
