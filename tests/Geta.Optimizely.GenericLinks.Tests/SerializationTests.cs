// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using System.Xml.Linq;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void LinkHtmlSerializer_can_Serialize()
        {
            var link = CreateLinkData("Test", "http://localhost/test", "_blank", "Test title");
            var subject = CreateLinkHtmlSerializer();

            var serialized = subject.Serialize(link, StringMode.InternalMode);
            Assert.NotNull(subject);

            var rootElement = XElement.Parse(serialized);
            var listElements = rootElement.Elements("li");
            Assert.Single(listElements);

            var listElement = listElements.First();
            var linkElements = listElement.Elements("a");
            Assert.Single(linkElements);

            var linkElement = linkElements.First();
            Assert.Equal(3, linkElement.Attributes().Count());

            var elementText = GetText(linkElement);
            Assert.Equal(link.Text, elementText);

            var hrefAttribute = linkElement.Attribute("href");
            Assert.NotNull(hrefAttribute);
            if (hrefAttribute is null)
                throw new InvalidOperationException("href cannot be null here");

            Assert.Equal(link.Href, hrefAttribute.Value);

            var targetAttribute = linkElement.Attribute("target");
            Assert.NotNull(targetAttribute);
            if (targetAttribute is null)
                throw new InvalidOperationException("target cannot be null here");

            Assert.Equal(link.Target, targetAttribute.Value);

            var titleAttribute = linkElement.Attribute("title");
            Assert.NotNull(titleAttribute);
            if (titleAttribute is null)
                throw new InvalidOperationException("title cannot be null here");

            Assert.Equal(link.Title, titleAttribute.Value);
        }

        private static string GetText(XElement element)
        {
            if (element.FirstNode is not XText text)
                return string.Empty;

            return text.Value;
        }

        private static TestLinkData CreateLinkData(string text, string href, string? target = null, string? title = null)
        {
            return new TestLinkData
            {
                Text = text,
                Href = href,
                Target = target,
                Title = title
            };
        }

        private static DefaultLinkHtmlSerializer CreateLinkHtmlSerializer()
        {
            var urlResolver = new FakeUrlResolver();
            var virtualPathResolver = new FakeVirtualPathResolver();

            return new DefaultLinkHtmlSerializer(virtualPathResolver, urlResolver);
        }
    }
}
