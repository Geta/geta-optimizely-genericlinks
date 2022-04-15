// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class PropertyLinkDataTests
    {
        [Fact]
        public void PropertyLinkData_can_Clone()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var property = CreatePropertyLinkData(link);

            var subject = (PropertyTestLinkData)property.CreateWritableClone();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            Assert.Equal(link.Text, subject.Link.Text);
            Assert.Equal(link.Href, subject.Link.Href);
            Assert.Equal(link.Target, subject.Link.Target);
            Assert.Equal(link.Title, subject.Link.Title);
        }

        [Fact]
        public void PropertyLinkData_can_Load()
        {
            var link = CreateLinkData("1", "http://localhost/1");
            var subject = CreatePropertyLinkData(null);

            subject.LoadData(link);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Link);

            if (subject.Link is null)
                throw new InvalidOperationException("subject link cannot be null here");

            Assert.Equal(link.Text, subject.Link.Text);
            Assert.Equal(link.Href, subject.Link.Href);
            Assert.Equal(link.Target, subject.Link.Target);
            Assert.Equal(link.Title, subject.Link.Title);

            var serialized = subject.GetBackingValue();
            Assert.NotNull(serialized);

            subject.LoadData(serialized);

            Assert.Equal(link.Text, subject.Link.Text);
            Assert.Equal(link.Href, subject.Link.Href);
            Assert.Equal(link.Target, subject.Link.Target);
            Assert.Equal(link.Title, subject.Link.Title);
        }

        [Fact]
        public void PropertyLinkDataCollection_can_Clone()
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

            Assert.Equal(firstLink.Text, subject.Links[0].Text);
            Assert.Equal(firstLink.Href, subject.Links[0].Href);
            Assert.Equal(firstLink.Target, subject.Links[0].Target);
            Assert.Equal(firstLink.Title, subject.Links[0].Title);

            Assert.Equal(secondLink.Text, subject.Links[1].Text);
            Assert.Equal(secondLink.Href, subject.Links[1].Href);
            Assert.Equal(secondLink.Target, subject.Links[1].Target);
            Assert.Equal(secondLink.Title, subject.Links[1].Title);
        }

        [Fact]
        public void PropertyLinkDataCollection_can_Load()
        {
            var firstLink = CreateLinkData("1", "http://localhost/1");
            var secondLink = CreateLinkData("2", "http://localhost/2");

            var links = CreateTestLinkDataCollection(firstLink, secondLink);
            var subject = CreatePropertyLinkDataCollection();

            subject.LoadData(links);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Links);

            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            Assert.Equal(2, subject.Links.Count);

            Assert.Equal(firstLink.Text, subject.Links[0].Text);
            Assert.Equal(firstLink.Href, subject.Links[0].Href);
            Assert.Equal(firstLink.Target, subject.Links[0].Target);
            Assert.Equal(firstLink.Title, subject.Links[0].Title);

            Assert.Equal(secondLink.Text, subject.Links[1].Text);
            Assert.Equal(secondLink.Href, subject.Links[1].Href);
            Assert.Equal(secondLink.Target, subject.Links[1].Target);
            Assert.Equal(secondLink.Title, subject.Links[1].Title);

            var serialized = subject.GetBackingValue();
            Assert.NotNull(serialized);

            subject.LoadData(serialized);

            Assert.Equal(2, subject.Links.Count);

            Assert.Equal(firstLink.Text, subject.Links[0].Text);
            Assert.Equal(firstLink.Href, subject.Links[0].Href);
            Assert.Equal(firstLink.Target, subject.Links[0].Target);
            Assert.Equal(firstLink.Title, subject.Links[0].Title);

            Assert.Equal(secondLink.Text, subject.Links[1].Text);
            Assert.Equal(secondLink.Href, subject.Links[1].Href);
            Assert.Equal(secondLink.Target, subject.Links[1].Target);
            Assert.Equal(secondLink.Title, subject.Links[1].Title);
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

            subject.Links = CreateTestLinkDataCollection(firstLink, secondLink, thirdLink);
            Assert.True(subject.IsModified);

            subject.MakeReadOnly();
            subject = (PropertyTestCollection)property.CreateWritableClone();
            Assert.False(subject.IsModified);

            if (subject.Links is null)
                throw new InvalidOperationException("subject links cannot be null here");

            subject.Links[1].Text = "Changed text";
            Assert.True(subject.IsModified);
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

        private static PropertyTestCollection CreatePropertyLinkDataCollection(params TestLinkData[] linkData)
        {
            var urlResolver = new FakeUrlResolver();
            var attributeSanitizer = new DefaultAttributeSanitizer();
            var virtualPathResolver = new FakeVirtualPathResolver();
            var linkSerializer = new DefaultLinkHtmlSerializer(virtualPathResolver, urlResolver);
            var collection = CreateTestLinkDataCollection(linkData);

            return new PropertyTestCollection(collection, urlResolver, attributeSanitizer, linkSerializer);
        }

        private static LinkDataCollection<TestLinkData> CreateTestLinkDataCollection(params TestLinkData[] linkData)
        {
            return new LinkDataCollection<TestLinkData>(linkData);
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
