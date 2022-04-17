// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Shell;
using Geta.Optimizely.GenericLinks.Cms.EditorDescriptors;
using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Geta.Optimizely.GenericLinks.Converters;
using Geta.Optimizely.GenericLinks.Converters.Attributes;
using Geta.Optimizely.GenericLinks.Converters.Json;
using Geta.Optimizely.GenericLinks.Converters.Values;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void AttributeConverters_can_Convert()
        {
            ILinkDataAttibuteConverter subject = new StringAttributeConverter();

            Assert.True(subject.CanConvert(typeof(string)));
            Assert.Equal("test", subject.Convert("test"));

            subject = new ConvertibleAttributeConverter();

            Assert.True(subject.CanConvert(typeof(int)));
            Assert.Equal("1", subject.Convert(1));
            Assert.True(subject.CanConvert(typeof(double)));
            Assert.Equal("1.1", subject.Convert(1.1));
            Assert.True(subject.CanConvert(typeof(DateTime)));
            Assert.Equal("01/01/2000 00:00:00", subject.Convert(new DateTime(2000, 1, 1)));

            subject = new JsonAttributeConverter();

            Assert.True(subject.CanConvert(typeof(DialogContentOptions)));
            Assert.Contains("test", subject.Convert(new DialogContentOptions { BaseClass = "test" }));           
        }


        [Fact]
        public void ValueWriters_can_Write()
        {
            ILinkDataAttibuteConverter subject = new StringAttributeConverter();

            Assert.True(subject.CanConvert(typeof(string)));
            Assert.Equal("test", subject.Convert("test"));

            subject = new ConvertibleAttributeConverter();

            Assert.True(subject.CanConvert(typeof(int)));
            Assert.Equal("1", subject.Convert(1));
            Assert.True(subject.CanConvert(typeof(double)));
            Assert.Equal("1.1", subject.Convert(1.1));
            Assert.True(subject.CanConvert(typeof(DateTime)));
            Assert.Equal("01/01/2000 00:00:00", subject.Convert(new DateTime(2000, 1, 1)));

            subject = new JsonAttributeConverter();

            Assert.True(subject.CanConvert(typeof(DialogContentOptions)));
            Assert.Contains("test", subject.Convert(new DialogContentOptions { BaseClass = "test" }));
        }

        [Fact]
        public void SystemTextLinkDataConverter_can_Write()
        {
            var model = new TestThumbnailLinkData
            {
                Text = "1",
                Href = "http://localhost/1",
                Thumbnail = new ContentReference(1),
                ThumbnailModified = new DateTime(2000, 1, 1),
                ThumbnailWidth = 256,
                ThumbnailHeight = 256,
            };

            var subject = CreateSystemTextLinkDataConverter<TestThumbnailLinkData>();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };

            using var memoryStream = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(memoryStream);

            subject.Write(jsonWriter, model, options);
            jsonWriter.Flush();
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            
            using var reader = new StreamReader(memoryStream, leaveOpen: true);
            var json = reader.ReadToEnd();

            Assert.NotNull(json);
            Assert.Contains(model.Text, json);
            Assert.Contains(model.Href, json);
        }

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

        private static SystemTextLinkDataConverter<TLinkData> CreateSystemTextLinkDataConverter<TLinkData>()
            where TLinkData : LinkData, new()
        {
            var propertyReflector = new DefaultPropertyReflector();
            var linkModelConverter = CreateLinkModelConverter();
            var valueWriters = CreateValueWriters();

            return new SystemTextLinkDataConverter<TLinkData>(propertyReflector, linkModelConverter, valueWriters);
        }

        private static DefaultLinkModelConverter CreateLinkModelConverter()
        {
            var attributeConverters = CreateAttributeConverters();
            var urlResolver = new FakeUrlResolver();
            var virtualPathResolver = new FakeVirtualPathResolver();
            var frameRepository = new InMemoryFrameRepository(CreateSystemFrames());
            var uiDescriptorRepository = CreateUiDescriptorRegistry();

            return new DefaultLinkModelConverter(urlResolver, frameRepository, virtualPathResolver, attributeConverters, uiDescriptorRepository);
        }

        private static UIDescriptorRegistry CreateUiDescriptorRegistry()
        {
            var descriptors = Enumerable.Empty<UIDescriptor>();
#pragma warning disable CS0618 // Type or member is obsolete
            var initializers = Enumerable.Empty<IUIDescriptorInitializer>();
#pragma warning restore CS0618 // Type or member is obsolete
            var providers = Enumerable.Empty<UIDescriptorProvider>();

            return new UIDescriptorRegistry(descriptors, initializers, providers);
        }

        private static IEnumerable<Frame> CreateSystemFrames()
        {
            yield return new Frame(1, "_blank", "Open in new window", true);
            yield return new Frame(2, "_top", "Open in full body", true);
        }

        private static IEnumerable<ILinkDataAttibuteConverter> CreateAttributeConverters()
        {
            yield return new StringAttributeConverter();
            yield return new ConvertibleAttributeConverter();
            yield return new JsonAttributeConverter();
        }

        private static IEnumerable<ILinkDataValueWriter> CreateValueWriters()
        {
            yield return new StringValueWriter();
            yield return new ContentReferenceValueWriter();
            yield return new Int32ValueWriter();
            yield return new DoubleValueWriter();
            yield return new DecimalValueWriter();
            yield return new DateTimeValueWriter();
        }
    }
}
