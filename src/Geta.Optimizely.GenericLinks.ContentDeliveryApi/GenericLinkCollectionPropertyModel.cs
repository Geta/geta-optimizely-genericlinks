// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using EPiServer.ContentApi.Core.Serialization;
using EPiServer.ContentApi.Core.Serialization.Models;
using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Extensions;

namespace Geta.Optimizely.GenericLinks.ContentDeliveryApi
{
    public class GenericLinkCollectionPropertyModel<T> : PropertyModel<LinkDataCollection<T>, PropertyLinkDataCollection<T>>, IExpandableProperty<LinkDataCollection<T>> where T : LinkData, new()
    {
        private readonly IUrlResolver _urlResolver;

        public GenericLinkCollectionPropertyModel(PropertyLinkDataCollection<T> propertyLinkData, IUrlResolver urlResolver) : base(propertyLinkData)
        {
            _urlResolver = urlResolver;
            Value = GetValue(propertyLinkData?.Links);

            ExpandedValue = Value;
        }

        public virtual LinkDataCollection<T> ExpandedValue { get; set; }

        public virtual void Expand(CultureInfo language)
        {
        }

        private LinkDataCollection<T> GetValue(LinkDataCollection<T> links)
        {
            if (links is null)
                return null;

            foreach (var link in links)
            {
                link.Href = _urlResolver.GetUrl(link.GetMappedHref());
            }

            return links;
        }
    }
}
