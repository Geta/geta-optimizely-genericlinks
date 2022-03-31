using EPiServer.Core;
using EPiServer.DataAbstraction;
using Geta.GenericLinks.Cms.Registration;
using Geta.GenericLinks.Tests.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Geta.GenericLinks.Tests
{
    public class BackingTypeResolverTests
    {
        [Fact]
        public void LinkDataBackingTypeResolverInterceptor_can_Resolve()
        {
            var testFailureException = new InvalidOperationException("Test failed");
            var failureResolver = new ExceptionBackingTypeResolver(testFailureException);

            var propertyType = typeof(PropertyThumbnailCollection);
            var propertyTypeName = propertyType.FullName;
            var propertyAssemblyName = propertyType.Assembly.FullName;
            var definitions = new List<PropertyDefinitionType>
            {
                new PropertyDefinitionType(1, PropertyDataType.LinkCollection, "Thumbnail collection", propertyTypeName, propertyAssemblyName)
            };

            var definitionRepository = new InMemoryPropertyDefinitionTypeRepository(definitions);
            var subject = new LinkDataBackingTypeResolverInterceptor(failureResolver, definitionRepository);

            var resolvedType = subject.Resolve(typeof(LinkDataCollection<ThumbnailLinkData>));

            Assert.Equal(propertyType, resolvedType);
        }
    }
}
