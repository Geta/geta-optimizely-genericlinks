using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Initialization.Internal;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using Geta.GenericLinks.Cms.EditorModels;
using Geta.GenericLinks.Cms.Metadata;
using Geta.GenericLinks.Cms.Registration;
using Geta.GenericLinks.Converters;
using Geta.GenericLinks.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
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

            services.AddSingleton<PropertyLinkDataDefinitionsLoader>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<JsonConverter, LinkDataConverter>());
            services.TryAddTransient<ILinkModelMetadataProvider, DefaultLinkModelMetadataProvider>();
            services.TryAddSingleton<IPropertyReflector, DefaultPropertyReflector>();
            services.TryAddTransient<IAttributeSanitizer, DefaultAttributeSanitizer>();
            services.TryAddTransient<ILinkHtmlSerializer, DefaultLinkHtmlSerializer>();

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
            var definitionLoader = provider.GetInstance<PropertyLinkDataDefinitionsLoader>();

            var editorType = typeof(LinkModel<>);
            var collectionType = typeof(LinkDataCollection<>);

            foreach (var type in definitionLoader.Load())
            {
                var editModelType = editorType.MakeGenericType(type);
                var propertyModelType = collectionType.MakeGenericType(type);
                var linkExtender = new LinkDataMetadataExtender(type, provider.GetAllInstances<IContentRepositoryDescriptor>());

                metadataHandlerRegistry.RegisterMetadataHandler(propertyModelType, linkExtender);
                metadataHandlerRegistry.RegisterMetadataHandler(editModelType, provider.GetInstance<ILinkModelMetadataProvider>());
            }
        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}
