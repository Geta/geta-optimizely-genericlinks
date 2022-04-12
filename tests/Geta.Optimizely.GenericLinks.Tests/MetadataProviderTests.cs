// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Geta.Optimizely.GenericLinks.Tests.Models;
using System;
using System.Linq;
using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
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

            var metadata = subject.GetMetadataForProperties(testInstance, testInstance.GetType());

            Assert.NotNull(metadata);

            var metaArray = metadata.ToArray();

            Assert.Equal(10, metaArray.Length);

            var firstThumbnail = metaArray[0];
            var secondThumbnail = metaArray[3];
            var thirdThumbnail = metaArray[6];

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
