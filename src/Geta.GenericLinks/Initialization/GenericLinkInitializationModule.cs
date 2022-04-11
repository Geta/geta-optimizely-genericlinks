using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Initialization.Internal;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Json;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using Geta.GenericLinks.Cms.EditorModels;
using Geta.GenericLinks.Cms.Metadata;
using Geta.GenericLinks.Cms.Registration;
using Geta.GenericLinks.Converters;
using Geta.GenericLinks.Converters.Attributes;
using Geta.GenericLinks.Converters.Json;
using Geta.GenericLinks.Converters.Values;
using Geta.GenericLinks.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

namespace Geta.GenericLinks.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    [ModuleDependency(typeof(CmsRuntimeInitialization))]
    internal class GenericLinkInitializationModule : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddSingleton<PropertyLinkDataCollectionDefinitionsLoader>();
            services.AddSingleton<PropertyLinkDataDefinitionsLoader>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ContentScannerExtension, GenericLinkContentScannerExtension>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<JsonConverter, NewtonsoftLinkDataConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IJsonConverter, SystemTextLinkDataJsonConverterFactory>());
            services.TryAddTransient<ILinkModelConverter, DefaultLinkModelConverter>();
            services.TryAddTransient<ILinkModelMetadataProvider, DefaultLinkModelMetadataProvider>();
            services.TryAddSingleton<IPropertyReflector, DefaultPropertyReflector>();
            services.TryAddTransient<IAttributeSanitizer, DefaultAttributeSanitizer>();
            services.TryAddTransient<ILinkHtmlSerializer, DefaultLinkHtmlSerializer>();

            TryAddAttributeConverters(services);
            TryAddJsonValueWriters(services);

            context.ConfigurationComplete += (o, e) =>
            {
                services.Intercept<IBackingTypeResolver>((provider, typeResolver) =>
                    new LinkDataBackingTypeResolverInterceptor(typeResolver, provider.GetRequiredService<IPropertyDefinitionTypeRepository>()));
            };
        }
        
        public void Initialize(InitializationEngine context)
        {
            var provider = context.Locate.Advanced;
            var metadataHandlerRegistry = provider.GetInstance<MetadataHandlerRegistry>();
            var collectionDefinitionLoader = provider.GetInstance<PropertyLinkDataCollectionDefinitionsLoader>();
            var propertyDefinitionLoader = provider.GetInstance<PropertyLinkDataDefinitionsLoader>();

            var editorType = typeof(LinkModel<>);
            var collectionType = typeof(LinkDataCollection<>);
            var editModelTypes = new HashSet<Type>();

            foreach (var resolvedType in collectionDefinitionLoader.Load())
            {
                var editModelType = editorType.MakeGenericType(resolvedType);
                var collectionModelType = collectionType.MakeGenericType(resolvedType);
                var collectionExtender = new LinkDataMetadataExtender(resolvedType, false, provider.GetAllInstances<IContentRepositoryDescriptor>());

                metadataHandlerRegistry.RegisterMetadataHandler(collectionModelType, collectionExtender);
                metadataHandlerRegistry.RegisterMetadataHandler(editModelType, provider.GetInstance<ILinkModelMetadataProvider>());

                editModelTypes.Add(editModelType);
            }

            foreach (var resolvedType in propertyDefinitionLoader.Load())
            {
                var editModelType = editorType.MakeGenericType(resolvedType);
                var singleItemExtender = new LinkDataMetadataExtender(resolvedType, true, provider.GetAllInstances<IContentRepositoryDescriptor>());

                metadataHandlerRegistry.RegisterMetadataHandler(resolvedType, singleItemExtender);

                if (editModelTypes.Contains(editModelType))
                    continue;

                metadataHandlerRegistry.RegisterMetadataHandler(editModelType, provider.GetInstance<ILinkModelMetadataProvider>());
            }
        }

        public void Uninitialize(InitializationEngine context)
        {

        }

        private static void TryAddAttributeConverters(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataAttibuteConverter, StringAttributeConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataAttibuteConverter, ConvertibleAttributeConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataAttibuteConverter, JsonAttributeConverter>());
        }

        private static void TryAddJsonValueWriters(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, StringValueWriter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, ContentReferenceValueWriter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, Int32ValueWriter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, DoubleValueWriter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, DecimalValueWriter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, DateTimeValueWriter>());
        }       
    }
}
