// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using EPiServer.ContentApi.Core.Serialization;
using EPiServer.ContentApi.Core.Serialization.Models;
using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Extensions;

namespace Geta.Optimizely.GenericLinks.ContentDeliveryApi;

public class GenericLinkPropertyModel<T> : PropertyModel<T, PropertyLinkData<T>>, IExpandableProperty<T> where T : LinkData, new()
{
    private readonly IUrlResolver _urlResolver;

    public GenericLinkPropertyModel(PropertyLinkData<T> propertyLinkData, IUrlResolver urlResolver) : base(propertyLinkData)
    {
        _urlResolver = urlResolver;
        Value = GetValue(propertyLinkData?.Link);
        ExpandedValue = Value;
    }

    public virtual T ExpandedValue { get; set; }

    public virtual void Expand(CultureInfo language)
    {
    }        

    private T GetValue(T link)
    {
        if (link is null)
            return null;

        link.Href = _urlResolver.GetUrl(link.GetMappedHref());
        return link;
    }
}
