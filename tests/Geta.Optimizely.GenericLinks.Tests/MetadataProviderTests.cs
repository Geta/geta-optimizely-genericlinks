// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Framework.Localization;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Geta.Optimizely.GenericLinks.Tests.Models;
using System;
using System.Linq;
using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests;

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
        Assert.True(metadata.Count() > 7);
    }

    [Fact]
    public void LinkModelMetadataProvider_sorts_Metadata_for_Properties()
    {
        var subject = CreateLinkModelMetadataProvider();
        var testInstance = new LinkModel<TestThumbnailLinkData>();

        var metadata = subject.GetMetadataForProperties(testInstance, testInstance.GetType());

        Assert.NotNull(metadata);

        var metaArray = metadata.ToArray();

        for (var i = 1; i < metaArray.Length; i++)
        {
            var first = metaArray[i - 1];
            var second = metaArray[i];

            Assert.True(first.Order <= second.Order);
        }
    }

    [Fact]
    public void LinkModelMetadataExtender_extends_single()
    {
        var provider = CreateExtensibleModelMetadataProvider();
        var attributes = Enumerable.Empty<Attribute>();
        var testType = typeof(TestLinkData);

        var metadata = provider.GetExtendedMetadataForType(testType, () => null);
        var subject = CreateLinkDataMetadataExtender(testType, true);
        
        subject.ModifyMetadata(metadata, attributes);

        Assert.NotNull(metadata);
        Assert.NotNull(metadata.ClientEditingClass);
        Assert.StartsWith("genericLinks", metadata.ClientEditingClass);
        Assert.EndsWith("GenericItemEditor", metadata.ClientEditingClass);

        Assert.NotNull(metadata.OverlayConfiguration);
        Assert.NotNull(metadata.EditorConfiguration);
    }

    [Fact]
    public void LinkModelMetadataExtender_extends_collection()
    {
        var provider = CreateExtensibleModelMetadataProvider();
        var attributes = Enumerable.Empty<Attribute>();
        var testType = typeof(TestLinkData);

        var metadata = provider.GetExtendedMetadataForType(testType, () => null);
        var subject = CreateLinkDataMetadataExtender(testType, false);

        subject.ModifyMetadata(metadata, attributes);

        Assert.NotNull(metadata);
        Assert.NotNull(metadata.ClientEditingClass);
        Assert.StartsWith("genericLinks", metadata.ClientEditingClass);
        Assert.EndsWith("GenericCollectionEditor", metadata.ClientEditingClass);

        Assert.NotNull(metadata.OverlayConfiguration);
        Assert.NotNull(metadata.EditorConfiguration);
    }

    private static LinkDataMetadataExtender CreateLinkDataMetadataExtender(Type extenderType, bool singleItem)
    {
        var descriptors = Enumerable.Empty<IContentRepositoryDescriptor>();
        return new LinkDataMetadataExtender(extenderType, singleItem, descriptors);
    }

    private static DefaultLinkModelMetadataProvider CreateLinkModelMetadataProvider()
    {
        var localizationService = LocalizationService.Current;
        var metadataHandlerRegistry = CreateMetadataHandlerRegistry();
        var compositeDetailsProvider = new NullCompositeMetadataDetailsProvider();
        var propertyReflector = new DefaultPropertyReflector();
        var validationAttributeAdapter = new FakeValidationAttributeAdapterProvider();
        var metadataProvider = new FakeModelMetadataProvider(compositeDetailsProvider, propertyReflector);
        var extensibleMetaProvider = new ExtensibleMetadataProvider(metadataHandlerRegistry, localizationService, metadataProvider, validationAttributeAdapter);

        return new DefaultLinkModelMetadataProvider(extensibleMetaProvider, localizationService, metadataHandlerRegistry, compositeDetailsProvider, validationAttributeAdapter, propertyReflector);
    }

    private static ExtensibleMetadataProvider CreateExtensibleModelMetadataProvider()
    {
        var localizationService = LocalizationService.Current;
        var metadataHandlerRegistry = CreateMetadataHandlerRegistry();
        var compositeDetailsProvider = new NullCompositeMetadataDetailsProvider();
        var propertyReflector = new DefaultPropertyReflector();
        var validationAttributeAdapter = new FakeValidationAttributeAdapterProvider();
        var metadataProvider = new FakeModelMetadataProvider(compositeDetailsProvider, propertyReflector);
        return new ExtensibleMetadataProvider(metadataHandlerRegistry, localizationService, metadataProvider, validationAttributeAdapter);
    }

    private static MetadataHandlerRegistry CreateMetadataHandlerRegistry()
    {
        var editorDescriptors = Enumerable.Empty<EditorDescriptor>();
        var modelAccessorCreators = Enumerable.Empty<IModelAccessorCreator>();
        var editorDefinitionRepository = new NullEditorDefinitionRepository();
        var metadataHandlerRegistry = new MetadataHandlerRegistry(editorDescriptors, modelAccessorCreators, editorDefinitionRepository);

        return metadataHandlerRegistry;
    }
}
