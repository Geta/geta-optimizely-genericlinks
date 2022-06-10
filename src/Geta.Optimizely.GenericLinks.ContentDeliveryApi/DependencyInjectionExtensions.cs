// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ContentApi.Core.Serialization;
using Geta.Optimizely.GenericLinks.ContentDeliveryApi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static void AddGenericLinkConverters(this IServiceCollection services)
        {
            services.AddSingleton<IPropertyConverterProvider>(provider => new GenericLinkConverterProvider(provider));
            services.AddSingleton<IPropertyConverterProvider>(provider => new GenericLinkCollectionConverterProvider(provider));
        }
    }
}
