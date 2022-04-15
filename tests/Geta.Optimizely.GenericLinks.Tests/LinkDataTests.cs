// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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

            var subjectFirstLink = subject[0];

            Assert.Equal(firstLink.Text, subjectFirstLink.Text);
            Assert.Equal(firstLink.Href, subjectFirstLink.Href);

            var subjectSecondLink = subject[1];

            Assert.Equal(secondLink.Text, subjectSecondLink.Text);
            Assert.Equal(secondLink.Href, subjectSecondLink.Href);
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

            collection.MakeReadOnly();

            var subject = collection.CreateWritableClone();

            Assert.NotNull(subject);
            Assert.False(subject.IsModified);

            subject.Add(thirdLink);
            Assert.Equal(3, subject.Count);
            Assert.True(subject.IsModified);

            subject.MakeReadOnly();
            subject = subject.CreateWritableClone();
            Assert.False(subject.IsModified);

            subject.RemoveAt(2);
            Assert.Equal(2, subject.Count);
            Assert.True(subject.IsModified);

            subject.MakeReadOnly();
            subject = subject.CreateWritableClone();
            Assert.False(subject.IsModified);

            subject[0].Href = thirdLink.Href;
            Assert.True(subject.IsModified);

            subject.MakeReadOnly();
            subject = subject.CreateWritableClone();
            Assert.False(subject.IsModified);

            subject.Clear();
            Assert.Empty(subject);
            Assert.True(subject.IsModified);
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
