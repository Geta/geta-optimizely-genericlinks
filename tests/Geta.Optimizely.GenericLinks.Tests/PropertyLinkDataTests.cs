// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class PropertyLinkDataTests
    {
        [Fact]
        public void PropertyLinkData_can_Construct()
        {
            var subject = CreatePropertyLinkData(null);
            Assert.NotNull(subject);

            var link = CreateLinkData("1", "http://localhost/1");
            subject = CreatePropertyLinkData(link);
            Assert.NotNull(subject);

            ConfigureScopedServiceLocator();

            try
            {
                subject = new PropertyTestLinkData();
                Assert.NotNull(subject);
                
                subject = new PropertyTestLinkData(link);
                Assert.NotNull(subject);                
            }
            finally
            {
                ClearScopedServiceLocator();
            }
        }

        [Fact]
        public void PropertyLinkDataCollection_can_Construct()
        {
            var subject = CreatePropertyLinkDataCollection();
            Assert.NotNull(subject);

            var link = CreateLinkData("1", "http://localhost/1");
            subject = CreatePropertyLinkDataCollection(link);
            Assert.NotNull(subject);

            ConfigureScopedServiceLocator();

            try
            {
                subject = new PropertyTestCollection();
                Assert.NotNull(subject);

                var collection = CreateLinkDataCollection(link);
                subject = new PropertyTestCollection(collection);
                Assert.NotNull(subject);
            }
            finally
            {
                ClearScopedServiceLocator();
            }
        }

        [Fact]
        public void PropertyLinkData_can_Clone_and_Copy()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var property = CreatePropertyLinkData(link);

            var subject = (PropertyTestLinkData)property.CreateWritableClone();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            void AssertEqual(TestLinkData expected, TestLinkData actual)
            {
                Assert.Equal(expected.Text, actual.Text);
                Assert.Equal(expected.Href, actual.Href);
                Assert.Equal(expected.Target, actual.Target);
                Assert.Equal(expected.Title, actual.Title);
            }

            AssertEqual(link, subject.Link);

            subject = (PropertyTestLinkData)property.Copy();

            Assert.NotNull(subject.Link);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            AssertEqual(link, subject.Link);
        }

        [Fact]
        public void PropertyLinkData_can_Load_and_Parse()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var subject = CreatePropertyLinkData(null);

            subject.LoadData(link);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            void AssertEqual(TestLinkData expected, TestLinkData actual)
            {
                Assert.Equal(expected.Text, actual.Text);
                Assert.Equal(expected.Href, actual.Href);
                Assert.Equal(expected.Target, actual.Target);
                Assert.Equal(expected.Title, actual.Title);
            }

            AssertEqual(link, subject.Link);

            var serialized = subject.ToBackingValue();
            Assert.NotNull(serialized);

            subject.LoadData(serialized);

            AssertEqual(link, subject.Link);

            serialized = subject.GetBackingProperty();
            subject.ParseToSelf(serialized);

            AssertEqual(link, subject.Link);
        }

        [Fact]
        public void PropertyLinkDataCollection_can_Clone_and_Copy()
        {
            var firstLink = CreateLinkData("1", "http://localhost/1");
            var secondLink = CreateLinkData("2", "http://localhost/2");

            var property = CreatePropertyLinkDataCollection(firstLink, secondLink);

            var subject = (PropertyTestCollection)property.CreateWritableClone();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);
            
            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            Assert.Equal(2, subject.Links.Count);

            void AssertEqual(TestLinkData expected, TestLinkData actual)
            {
                Assert.Equal(expected.Text, actual.Text);
                Assert.Equal(expected.Href, actual.Href);
                Assert.Equal(expected.Target, actual.Target);
                Assert.Equal(expected.Title, actual.Title);
            }

            AssertEqual(firstLink, subject.Links[0]);
            AssertEqual(secondLink, subject.Links[1]);

            subject = (PropertyTestCollection)property.Copy();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);

            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            AssertEqual(firstLink, subject.Links[0]);
            AssertEqual(secondLink, subject.Links[1]);
        }

        [Fact]
        public void PropertyLinkDataCollection_can_Load_and_Parse()
        {
            var firstLink = CreateLinkData("1", "http://localhost/1");
            var secondLink = CreateLinkData("2", "http://localhost/2");

            var links = CreateLinkDataCollection(firstLink, secondLink);
            var subject = CreatePropertyLinkDataCollection();

            subject.LoadData(links);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);

            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            Assert.Equal(2, subject.Links.Count);

            void AssertEqual(TestLinkData expected, TestLinkData actual)
            {
                Assert.Equal(expected.Text, actual.Text);
                Assert.Equal(expected.Href, actual.Href);
                Assert.Equal(expected.Target, actual.Target);
                Assert.Equal(expected.Title, actual.Title);
            }

            AssertEqual(firstLink, subject.Links[0]);
            AssertEqual(secondLink, subject.Links[1]);

            var serialized = subject.ToBackingValue();
            Assert.NotNull(serialized);

            subject.LoadData(serialized);

            Assert.Equal(2, subject.Links.Count);

            AssertEqual(firstLink, subject.Links[0]);
            AssertEqual(secondLink, subject.Links[1]);

            serialized = subject.GetBackingProperty();
            subject.ParseToSelf(serialized);

            Assert.Equal(2, subject.Links.Count);

            AssertEqual(firstLink, subject.Links[0]);
            AssertEqual(secondLink, subject.Links[1]);
        }

        [Fact]
        public void PropertyLinkData_updates_IsModified()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var property = CreatePropertyLinkData(link);
            property.MakeReadOnly();

            var subject = (PropertyTestLinkData)property.CreateWritableClone();
            
            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            Assert.False(subject.IsModified);
            
            subject.Link = CreateLinkData("2", "http://localhost/2");
            Assert.True(subject.IsModified);

            subject.MakeReadOnly();
            subject = (PropertyTestLinkData)property.CreateWritableClone();
            Assert.False(subject.IsModified);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            subject.Link.Text = "Changed text";
            Assert.True(subject.IsModified);
        }

        [Fact]
        public void PropertyLinkDataCollection_updates_IsModified()
        {
            var firstLink = CreateLinkData("1", "http://localhost/1");
            var secondLink = CreateLinkData("2", "http://localhost/2");
            var thirdLink = CreateLinkData("3", "http://localhost/3");
            var property = CreatePropertyLinkDataCollection(firstLink, secondLink);
            property.MakeReadOnly();

            var subject = (PropertyTestCollection)property.CreateWritableClone();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);

            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            Assert.False(subject.IsModified);

            subject.Links = CreateLinkDataCollection(firstLink, secondLink, thirdLink);
            Assert.True(subject.IsModified);

            subject.MakeReadOnly();
            subject = (PropertyTestCollection)property.CreateWritableClone();
            Assert.False(subject.IsModified);

            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            subject.Links[1].Text = "Changed text";
            Assert.True(subject.IsModified);
        }

        [Fact]
        public void PropertyLinkData_handles_null()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var subject = CreatePropertyLinkData(link);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);

            subject.Link = null;

            Assert.True(subject.IsNull);

            subject.Link = CreateLinkData("1", "");

            Assert.True(subject.IsNull);

            subject.ParseToSelf(null);

            Assert.True(subject.IsNull);

            subject.LoadData(null);

            Assert.True(subject.IsNull);
        }

        [Fact]
        public void PropertyLinkDataCollection_handles_null()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var subject = CreatePropertyLinkDataCollection(link);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);

            subject.Links = null;

            Assert.True(subject.IsNull);

            subject.Links = CreateLinkDataCollection(link);
            subject.Clear();

            Assert.True(subject.IsNull);

            subject.Links = CreateLinkDataCollection(link);
            subject.Links.Clear();

            Assert.True(subject.IsNull);

            subject.LoadData(null);

            Assert.True(subject.IsNull);

            subject.ParseToSelf(null);

            Assert.True(subject.IsNull);
        }

        [Fact]
        public void PropertyLinkData_can_RemapPermanentLinkReferences()
        {
            var initialLinkGuid = Guid.NewGuid();
            var linkData = CreateLinkData("Test", initialLinkGuid);
            var subject = CreatePropertyLinkData(linkData);

            var remappedLinkGuid = Guid.NewGuid();
            var mappings = new Dictionary<Guid, Guid>
            {
                { initialLinkGuid, remappedLinkGuid }
            };

            subject.RemapPermanentLinkReferences(mappings);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);
            Assert.NotNull(subject.ReferencedPermanentLinkIds);

            if (subject.Link is null)
                throw new InvalidOperationException("Link cannot be null");

            if (subject.ReferencedPermanentLinkIds is null)
                throw new InvalidOperationException("ReferencedPermanentLinkIds cannot be null");
            
            var mappedGuid = PermanentLinkUtility.GetGuid(subject.Link.Href);

            Assert.Equal(remappedLinkGuid, mappedGuid);
            Assert.Equal(1, subject.ReferencedPermanentLinkIds.Count);
            Assert.Equal(remappedLinkGuid, subject.ReferencedPermanentLinkIds[0]);
        }

        [Fact]
        public void PropertyLinkDataCollection_can_RemapPermanentLinkReferences()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var firstLinkData = CreateLinkData("Test 1", firstGuid);
            var secondLinkData = CreateLinkData("Test 2", secondGuid);

            var subject = CreatePropertyLinkDataCollection(firstLinkData, secondLinkData);

            var remappedGuids = new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid()
            };

            var mappings = new Dictionary<Guid, Guid>
            {
                { firstGuid, remappedGuids[0] },
                { secondGuid, remappedGuids[1] }
            };

            subject.RemapPermanentLinkReferences(mappings);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);

            if (subject.Links is null)
                throw new InvalidOperationException("Links cannot be null");

            if (subject.ReferencedPermanentLinkIds is null)
                throw new InvalidOperationException("ReferencedPermanentLinkIds cannot be null");

            for (var i = 0; i < subject.Links.Count; i++)
            {
                var subjectLinkItem = subject.Links[i];
                Assert.NotNull(subjectLinkItem.Href);

                var mappedGuid = PermanentLinkUtility.GetGuid(subjectLinkItem.Href);
                Assert.Equal(remappedGuids[i], mappedGuid);

                mappedGuid = subject.ReferencedPermanentLinkIds[i];
                Assert.Equal(remappedGuids[i], mappedGuid);
            }
        }

        private static PropertyTestLinkData CreatePropertyLinkData(TestLinkData? linkData)
        {
            var urlResolver = new FakeUrlResolver();
            var attributeSanitizer = new DefaultAttributeSanitizer();
            var virtualPathResolver = new FakeVirtualPathResolver();
            var linkSerializer = new DefaultLinkHtmlSerializer(virtualPathResolver, urlResolver);

            if (linkData is null)
                return new PropertyTestLinkData(urlResolver, attributeSanitizer, linkSerializer);

            return new PropertyTestLinkData(linkData, urlResolver, attributeSanitizer, linkSerializer);
        }

        private static void ConfigureScopedServiceLocator()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IUrlResolver, FakeUrlResolver>();
            serviceCollection.AddSingleton<IVirtualPathResolver, FakeVirtualPathResolver>();
            serviceCollection.AddSingleton<IAttributeSanitizer, DefaultAttributeSanitizer>();
            serviceCollection.AddSingleton<ILinkHtmlSerializer, DefaultLinkHtmlSerializer>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            ServiceLocator.SetScopedServiceProvider(serviceProvider);
        }

        private static void ClearScopedServiceLocator()
        {
            ServiceLocator.SetScopedServiceProvider(null);
        }

        private static PropertyTestCollection CreatePropertyLinkDataCollection(params TestLinkData[] linkData)
        {
            var urlResolver = new FakeUrlResolver();
            var attributeSanitizer = new DefaultAttributeSanitizer();
            var virtualPathResolver = new FakeVirtualPathResolver();
            var linkSerializer = new DefaultLinkHtmlSerializer(virtualPathResolver, urlResolver);
            var collection = CreateLinkDataCollection(linkData);

            return new PropertyTestCollection(collection, urlResolver, attributeSanitizer, linkSerializer);
        }

        private static LinkDataCollection<TestLinkData> CreateLinkDataCollection(params TestLinkData[] linkData)
        {
            return new LinkDataCollection<TestLinkData>(linkData);
        }

        private static TestLinkData CreateLinkData(string text, Guid contentGuid, string extension = ".aspx", string? target = null, string? title = null)
        {
            var linkUri = PermanentLinkUtility.GetPermanentLinkUrl(contentGuid, extension);
            return new TestLinkData
            {
                Text = text,
                Href = linkUri.ToString(),
                Target = target,
                Title = title
            };
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
    }
}
