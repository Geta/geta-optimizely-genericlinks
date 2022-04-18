// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections;
using Geta.Optimizely.GenericLinks.Extensions;
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
    }
}
