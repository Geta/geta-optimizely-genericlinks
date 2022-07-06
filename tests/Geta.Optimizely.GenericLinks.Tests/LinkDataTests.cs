// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPiServer.Web;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class LinkDataTests
    {
        [Fact]
        public void LinkData_can_Clone()
        {
            var text = "Link";
            var href = "https://getadigital.com/";
            var target = "_blank";
            var title = "A link";
            var ariaHidden = "false";

            var link = CreateLinkData(text, href, target, title);

            link.Attributes.Add("aria-hidden", ariaHidden);

            var subject = (LinkData)link.Clone();

            Assert.NotNull(subject);
            Assert.Equal(text, subject.Text);
            Assert.Equal(href, subject.Href);
            Assert.Equal(target, subject.Target);
            Assert.Equal(title, subject.Title);

            var hasHiddenAttribute = subject.Attributes.ContainsKey("aria-hidden");
            Assert.True(hasHiddenAttribute);

            var hiddenAttribute = subject.Attributes["aria-hidden"];
            Assert.Equal(ariaHidden, hiddenAttribute);
        }

        [Fact]
        public async Task LinkData_GetAttributeKey_is_thread_safe()
        {
            var key = "TestProperty";
            var subject = CreateLinkData(string.Empty, string.Empty);
            var taskCount = 1000;
            var tasks = new List<Task>(taskCount);

            for (var i = 0; i < taskCount; i++)
            {
                var task = Task.Run(() => subject.CallGetAttributeKey(key));
                tasks.Add(task);
            }
            
            await Task.WhenAll(tasks);

            Assert.All(tasks, x => Assert.True(x.IsCompleted));
        }

        [Fact]
        public void LinkData_can_RemapPermanentLinkReferences()
        {
            var initialLinkGuid = Guid.NewGuid();
            var subject = CreateLinkData("Test", initialLinkGuid);

            var remappedLinkGuid = Guid.NewGuid();
            var mappings = new Dictionary<Guid, Guid>
            {
                { initialLinkGuid, remappedLinkGuid }
            };

            subject.RemapPermanentLinkReferences(mappings);

            Assert.NotNull(subject);
            Assert.NotNull(subject.Href);

            var mappedGuid = PermanentLinkUtility.GetGuid(subject.Href);

            Assert.Equal(remappedLinkGuid, mappedGuid);
            Assert.Equal(1, subject.ReferencedPermanentLinkIds.Count);
            Assert.Equal(remappedLinkGuid, subject.ReferencedPermanentLinkIds[0]);
        }

        [Fact]
        public void LinkData_updates_IsModified()
        {
            var text = "Link";
            var href = "https://getadigital.com/";
            var target = "_blank";
            var title = "A link";
            var customAttribute = "320";

            var link = CreateLinkData(string.Empty, "http://localhost", null, null);
            link.SetModified(false);
            Assert.False(link.IsModified);
            
            link.Text = text;
            Assert.True(link.IsModified);
            link.SetModified(false);

            link.Href = href;
            Assert.True(link.IsModified);
            link.SetModified(false);

            link.Target = target;
            Assert.True(link.IsModified);
            link.SetModified(false);

            link.Title = title;
            Assert.True(link.IsModified);
            link.SetModified(false);

            link.CallSetAttribute("Custom", string.Empty);
            Assert.True(link.IsModified);
            link.SetModified(false);

            link.CallSetAttribute("Custom", customAttribute);
            Assert.True(link.IsModified);
        }

        [Fact]
        public void LinkDataCollection_can_Clone()
        {
            var firstLink = CreateLinkData("1", "http://localhost/1");
            var secondLink = CreateLinkData("2", "http://localhost/2");

            var collection = new LinkDataCollection<TestLinkData>
            {
                firstLink, secondLink
            };

            var subject = (LinkDataCollection<TestLinkData>)collection.Clone();

            Assert.NotNull(subject);
            Assert.Equal(collection.Count, subject.Count);

            void AssertEqual(TestLinkData expected, TestLinkData actual)
            {
                Assert.Equal(expected.Text, actual.Text);
                Assert.Equal(expected.Href, actual.Href);
            }

            AssertEqual(firstLink, subject[0]);
            AssertEqual(secondLink, subject[1]);
        }

        [Fact]
        public void LinkDataCollection_can_RemapPermanentLinkReferences()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var firstLinkData = CreateLinkData("Test 1", firstGuid);
            var secondLinkData = CreateLinkData("Test 2", secondGuid);

            var subject = new LinkDataCollection<TestLinkData>
            {
                firstLinkData, secondLinkData
            };

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

            for (var i = 0; i < subject.Count; i++)
            {
                var subjectLinkItem = subject[i];
                Assert.NotNull(subjectLinkItem.Href);

                var mappedGuid = PermanentLinkUtility.GetGuid(subjectLinkItem.Href);
                Assert.Equal(remappedGuids[i], mappedGuid);

                mappedGuid = subject.ReferencedPermanentLinkIds[i];
                Assert.Equal(remappedGuids[i], mappedGuid);
            }
        }

        [Fact]
        public void LinkDataCollection_updates_IsModified()
        {
            var firstLink = CreateLinkData("1", "http://localhost/1");
            var secondLink = CreateLinkData("2", "http://localhost/2");
            var thirdLink = CreateLinkData("3", "http://localhost/3");

            var collection = new LinkDataCollection<TestLinkData>
            {
                firstLink, secondLink
            };

            LinkDataCollection<TestLinkData> Reset(LinkDataCollection<TestLinkData> collection)
            {
                collection.MakeReadOnly();
                return collection.CreateWritableClone();
            }

            var subject = Reset(collection);

            Assert.NotNull(subject);
            Assert.False(subject.IsModified);

            subject.Add(thirdLink);
            Assert.Equal(3, subject.Count);
            Assert.True(subject.IsModified);

            subject = Reset(subject);
            Assert.False(subject.IsModified);

            subject.RemoveAt(2);
            Assert.Equal(2, subject.Count);
            Assert.True(subject.IsModified);

            subject = Reset(subject);
            Assert.False(subject.IsModified);

            subject[1] = thirdLink;
            Assert.True(subject.IsModified);

            subject = Reset(subject);
            Assert.False(subject.IsModified);

            subject[0].Href = thirdLink.Href;
            Assert.True(subject.IsModified);

            subject = Reset(subject);
            Assert.False(subject.IsModified);

            subject.Clear();
            Assert.Empty(subject);
            Assert.True(subject.IsModified);
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
