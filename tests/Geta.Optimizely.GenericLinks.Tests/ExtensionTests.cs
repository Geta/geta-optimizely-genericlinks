// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections;
using System.Xml.Linq;
using Geta.Optimizely.GenericLinks.Extensions;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class ExtensionTests
    {
        [Fact]
        public void Enumerable_Yield_yields()
        {
            var subject = 1;
            var enumerable = EnumerableExtensions.Yield(subject);

            Assert.Single(enumerable);
            Assert.True(typeof(IEnumerable).IsAssignableFrom(enumerable.GetType()));
        }

        [Fact]
        public void Enumerator_Empty_is_empty()
        {
            var subject = Enumerator.Empty();

            Assert.False(subject.MoveNext());

            subject = Enumerator.Empty<int>();

            Assert.False(subject.MoveNext());
        }

        [Fact]
        public void LinkDataExtensions_GetMappedHref_returns_href()
        {
            var virtualPathResolver = new FakeVirtualPathResolver();
            var linkData = new TestLinkData()
            {
                Href = "~/localhost/"
            };

            var subject = LinkDataExtensions.GetMappedHref(linkData, virtualPathResolver);

            Assert.Equal("localhost/", subject);
        }

        [Fact]
        public void LinkDataExtensions_ToMappedLink_returns_link()
        {
            var virtualPathResolver = new FakeVirtualPathResolver();
            var urlResolver = new FakeUrlResolver();
            var htmlSerializer = new DefaultLinkHtmlSerializer(virtualPathResolver, urlResolver);

            var linkData = new TestLinkData()
            {
                Href = "~/localhost/",
                Text = "Test",
                Title = "A test link"
            };

            var subject = LinkDataExtensions.ToMappedLink(linkData, virtualPathResolver, htmlSerializer);

            Assert.NotNull(subject);

            if (subject is null)
                throw new InvalidOperationException("subject cannot be null");

            var element = XElement.Parse(subject);

            var innerText = element.Value;
            Assert.Equal(linkData.Text, innerText);

            var elementType = element.Name.LocalName;
            Assert.Equal("a", elementType);

            var title = element.Attribute("title")?.Value;
            Assert.Equal(linkData.Title, title);

            var href = element.Attribute("href")?.Value;
            Assert.Equal("localhost/", href);
        }

        [Fact]
        public void LinkDataExtensions_ToPermanent_returns_link()
        {
            var virtualPathResolver = new FakeVirtualPathResolver();
            var urlResolver = new FakeUrlResolver();
            var htmlSerializer = new DefaultLinkHtmlSerializer(virtualPathResolver, urlResolver);

            var linkData = new TestLinkData()
            {
                Href = "~/localhost/",
                Text = "Test",
                Target = "_blank"
            };

            var subject = LinkDataExtensions.ToPermanentLink(linkData, urlResolver, htmlSerializer);

            Assert.NotNull(subject);

            if (subject is null)
                throw new InvalidOperationException("subject cannot be null");

            var element = XElement.Parse(subject);

            var innerText = element.Value;
            Assert.Equal(linkData.Text, innerText);

            var elementType = element.Name.LocalName;
            Assert.Equal("a", elementType);

            var target = element.Attribute("target")?.Value;
            Assert.Equal(linkData.Target, target);
        }
    }
}
