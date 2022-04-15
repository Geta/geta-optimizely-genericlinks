// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Html;

namespace Geta.Optimizely.GenericLinks.Tests.Models
{
    public class PropertyTestCollection : PropertyLinkDataCollection<TestLinkData>
    {
        public PropertyTestCollection(
            IUrlResolver urlResolver,
            IAttributeSanitizer attributeSanitizer,
            ILinkHtmlSerializer htmlSerializer)
            : base(urlResolver, attributeSanitizer, htmlSerializer)
        {
        }

        public PropertyTestCollection(
            LinkDataCollection<TestLinkData> linkItemCollection,
            IUrlResolver urlResolver,
            IAttributeSanitizer attributeSanitizer,
            ILinkHtmlSerializer htmlSerializer)
            : base(linkItemCollection, urlResolver, attributeSanitizer, htmlSerializer)
        {
        }

        public string GetBackingValue()
        {
            return LongString;
        }
    }
}
