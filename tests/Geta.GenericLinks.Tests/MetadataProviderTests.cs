using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using Geta.GenericLinks.Cms.EditorModels;
using Geta.GenericLinks.Cms.Metadata;
using Geta.GenericLinks.Tests.Models;
using Geta.GenericLinks.Tests.Services;
using System;
using System.Linq;
using Xunit;

namespace Geta.GenericLinks.Tests
{
    public class MetadataProviderTests
    {
        [Fact]
        public void LinkModelMetadataProvider_returns_Metadata()
        {
            var subject = CreateLinkModelMetadataProvider();

            var metadata = subject.CreateMetadata(Enumerable.Empty<Attribute>(), typeof(TestPage), null, typeof(TestLinkData), string.Empty);

            Assert.NotNull(metadata);
        }

        [Fact]
        public void LinkModelMetadataProvider_returns_Metadata_for_Properties()
        {
            var subject = CreateLinkModelMetadataProvider();
            var testInstance = new LinkModel<TestThumbnailLinkData>();

            var metadata = subject.GetMetadataForProperties(testInstance, testInstance.GetType());

            Assert.NotNull(metadata);
            Assert.Equal(10, metadata.Count());
        }

        [Fact]
        public void LinkModelMetadataProvider_sorts_Metadata_for_Properties()
        {
            var subject = CreateLinkModelMetadataProvider();
            var testInstance = new LinkModel<TestThumbnailLinkData>();

            var metadata = subject.GetMetadataForProperties(testInstance, testInstance.GetType()).ToArray();

            Assert.NotNull(metadata);
            Assert.Equal(10, metadata.Length);

            var firstThumbnail = metadata[0];
            var secondThumbnail = metadata[3];
            var thirdThumbnail = metadata[6];

            Assert.Equal(typeof(ContentReference), firstThumbnail.ModelType);
            Assert.Equal(nameof(TestThumbnailLinkData.ThumbnailOne), firstThumbnail.PropertyName);

            Assert.Equal(typeof(ContentReference), secondThumbnail.ModelType);
            Assert.Equal(nameof(TestThumbnailLinkData.ThumbnailTwo), secondThumbnail.PropertyName);

            Assert.Equal(typeof(ContentReference), thirdThumbnail.ModelType);
            Assert.Equal(nameof(TestThumbnailLinkData.ThumbnailThree), thirdThumbnail.PropertyName);
        }

        private static DefaultLinkModelMetadataProvider CreateLinkModelMetadataProvider()
        {
            var localizationService = LocalizationService.Current;
            var metadataHandlerRegistry = GetMetadataHandlerRegistry();
            var compositeDetailsProvider = new NullCompositeMetadataDetailsProvider();
            var propertyReflector = new DefaultPropertyReflector();
            var validationAttributeAdapter = new NullValidationAttributeAdapterProvider();
            var metadataProvider = new TestModelMetadataProvider(compositeDetailsProvider, propertyReflector);
            var extensibleMetaProvider = new ExtensibleMetadataProvider(metadataHandlerRegistry, localizationService, metadataProvider, validationAttributeAdapter);
            
            return new DefaultLinkModelMetadataProvider(extensibleMetaProvider, localizationService, metadataHandlerRegistry, compositeDetailsProvider, validationAttributeAdapter, propertyReflector);
        }

        private static MetadataHandlerRegistry GetMetadataHandlerRegistry()
        {
            var editorDescriptors = Enumerable.Empty<EditorDescriptor>();
            var modelAccessorCreators = Enumerable.Empty<IModelAccessorCreator>();
            var editorDefinitionRepository = new NullEditorDefinitionRepository();
            var metadataHandlerRegistry = new MetadataHandlerRegistry(editorDescriptors, modelAccessorCreators, editorDefinitionRepository);

            return metadataHandlerRegistry;
        }
    }
}
