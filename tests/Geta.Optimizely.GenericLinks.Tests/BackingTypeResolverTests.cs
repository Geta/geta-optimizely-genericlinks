// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.DataAbstraction;
using Geta.Optimizely.GenericLinks.Tests.Extensions;
using Geta.Optimizely.GenericLinks.Tests.Models;
using System;
using System.Collections.Generic;
using Geta.Optimizely.GenericLinks.Cms.Registration;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class BackingTypeResolverTests
    {
        [Fact]
        public void LinkDataBackingTypeResolverInterceptor_can_Resolve()
        {
            var testFailureException = new InvalidOperationException("Test failed");
            var failureResolver = new ExceptionBackingTypeResolver(testFailureException);

            var collectionPropertyType = typeof(PropertyTestCollection);
            var singlePropertyType = typeof(PropertyTestLinkData);
            var definitions = new List<PropertyDefinitionType>
            {
                collectionPropertyType.ToDefinition(1001, PropertyDataType.LinkCollection),
                singlePropertyType.ToDefinition(1002, PropertyDataType.LinkCollection)
            };

            var definitionRepository = new InMemoryPropertyDefinitionTypeRepository(definitions);
            var subject = new LinkDataBackingTypeResolverInterceptor(failureResolver, definitionRepository);

            var resolvedType = subject.Resolve(typeof(LinkDataCollection<TestLinkData>));
            Assert.Equal(collectionPropertyType, resolvedType);

            resolvedType = subject.Resolve(typeof(TestLinkData));
            Assert.Equal(singlePropertyType, resolvedType);
        }
    }
}
