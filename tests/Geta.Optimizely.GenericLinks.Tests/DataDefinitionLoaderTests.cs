using EPiServer.Core;
using EPiServer.DataAbstraction;
using Geta.Optimizely.GenericLinks.Tests.Extensions;
using Geta.Optimizely.GenericLinks.Tests.Models;
using System.Collections.Generic;
using System.Linq;
using Geta.Optimizely.GenericLinks.Cms.Registration;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class DataDefinitionLoaderTests
    {
        [Fact]
        public void PropertyLinkDataDefinitionsLoader_can_Load()
        {
            var definitions = new List<PropertyDefinitionType>
            {
                // Ids lower than 1000 are considered system types, belonging to Optimizely
                typeof(PropertyTestCollection).ToDefinition(1001, PropertyDataType.LinkCollection),
                typeof(PropertyTestThumbnailCollection).ToDefinition(1002, PropertyDataType.LinkCollection),

                typeof(PropertyBoolean).ToDefinition(1, PropertyDataType.Boolean),
                typeof(PropertyString).ToDefinition(2, PropertyDataType.String),
                typeof(PropertyNumber).ToDefinition(3, PropertyDataType.Number)
            };

            var definitionRepository = new InMemoryPropertyDefinitionTypeRepository(definitions);
            var subject = new PropertyLinkDataCollectionDefinitionsLoader(definitionRepository);

            var result = subject.Load();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
