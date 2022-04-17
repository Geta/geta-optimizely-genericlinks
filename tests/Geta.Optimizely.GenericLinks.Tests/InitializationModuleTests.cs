// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Geta.Optimizely.GenericLinks.Converters;
using Geta.Optimizely.GenericLinks.Converters.Attributes;
using Geta.Optimizely.GenericLinks.Converters.Values;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Initialization;
using Geta.Optimizely.GenericLinks.Tests.Extensions;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests
{
    public class InitializationModuleTests
    {
        [Fact]
        public void InitializationModule_can_ConfigureContainer()
        {
            var serviceCollection = new ServiceCollection();
            var context = new ServiceConfigurationContext(HostType.TestFramework, serviceCollection);
            var subject = new GenericLinkInitializationModule();
            
            subject.ConfigureContainer(context);

            Assert.Contains(serviceCollection, (d) => typeof(ILinkModelConverter).IsAssignableFrom(d.ServiceType));
            Assert.Contains(serviceCollection, (d) => typeof(ILinkModelMetadataProvider).IsAssignableFrom(d.ServiceType));
            Assert.Contains(serviceCollection, (d) => typeof(IPropertyReflector).IsAssignableFrom(d.ServiceType));
            Assert.Contains(serviceCollection, (d) => typeof(IAttributeSanitizer).IsAssignableFrom(d.ServiceType));
            Assert.Contains(serviceCollection, (d) => typeof(ILinkHtmlSerializer).IsAssignableFrom(d.ServiceType));
            Assert.Contains(serviceCollection, (d) => typeof(ILinkDataAttibuteConverter).IsAssignableFrom(d.ServiceType));
            Assert.Contains(serviceCollection, (d) => typeof(ILinkDataValueWriter).IsAssignableFrom(d.ServiceType));           
        }

        [Fact]
        public void InitializationModule_can_Initialize()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(LocalizationService.Current);
            serviceCollection.AddSingleton<ExtensibleMetadataProvider>();
            serviceCollection.AddSingleton<ICompositeMetadataDetailsProvider, NullCompositeMetadataDetailsProvider>();
            serviceCollection.AddSingleton<IValidationAttributeAdapterProvider, FakeValidationAttributeAdapterProvider>();
            serviceCollection.AddSingleton<IModelMetadataProvider, TestModelMetadataProvider>();
            serviceCollection.AddSingleton(CreateMetadataHandlerRegistry());
            serviceCollection.AddSingleton(CreatePropertyDefinitionTypeRepository());

            var configurationContext = new ServiceConfigurationContext(HostType.TestFramework, serviceCollection);
            var subject = new GenericLinkInitializationModule();
            
            subject.ConfigureContainer(configurationContext);

            var engine = CreateInitializationEngine(serviceCollection);

            subject.Initialize(engine);

            var handlerRegistry = engine.Locate.Advanced.GetService<MetadataHandlerRegistry>();
            
            Assert.NotNull(handlerRegistry);

            if (handlerRegistry is null)
                throw new InvalidOperationException("handler registry cannot be null");

            var singleHandler = handlerRegistry.GetMetadataHandlers(typeof(TestLinkData));
            Assert.NotEmpty(singleHandler);

            var collectionHandler = handlerRegistry.GetMetadataHandlers(typeof(LinkDataCollection<TestLinkData>));
            Assert.NotEmpty(collectionHandler);
        }

        [Fact]
        public void InitializationModule_can_Uninitialize()
        {
            var serviceCollection = new ServiceCollection();
            var configurationContext = new ServiceConfigurationContext(HostType.TestFramework, serviceCollection);
            var subject = new GenericLinkInitializationModule();

            subject.ConfigureContainer(configurationContext);

            var engine = CreateInitializationEngine(serviceCollection);
            
            subject.Uninitialize(engine);

            Assert.NotEmpty(serviceCollection);
        }

        private static InitializationEngine CreateInitializationEngine(IServiceCollection services)
        {
            var engine = new InitializationEngine(services, HostType.TestFramework);
            var provider = services.BuildServiceProvider();

            ServiceLocator.SetServiceProvider(provider);

            return engine;
        }

        private static MetadataHandlerRegistry CreateMetadataHandlerRegistry()
        {
            var editorDescriptors = Enumerable.Empty<EditorDescriptor>();
            var modelAccessorCreators = Enumerable.Empty<IModelAccessorCreator>();
            var editorDefinitionRepository = new NullEditorDefinitionRepository();
            var metadataHandlerRegistry = new MetadataHandlerRegistry(editorDescriptors, modelAccessorCreators, editorDefinitionRepository);

            return metadataHandlerRegistry;
        }
        private static IPropertyDefinitionTypeRepository CreatePropertyDefinitionTypeRepository()
        {
            var definitions = new List<PropertyDefinitionType>
            {
                // Ids lower than 1000 are considered system types, belonging to Optimizely
                typeof(PropertyTestCollection).ToDefinition(1001, PropertyDataType.LinkCollection),
                typeof(PropertyTestLinkData).ToDefinition(1002, PropertyDataType.LinkCollection),
                typeof(PropertyTestThumbnailCollection).ToDefinition(1003, PropertyDataType.LinkCollection)
            };

            return new InMemoryPropertyDefinitionTypeRepository(definitions);
        }
    }
}
