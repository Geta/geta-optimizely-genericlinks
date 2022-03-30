using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using Geta.GenericLinks.Cms.EditorModels;
using Geta.GenericLinks.Cms.Metadata;
using Geta.GenericLinks.Cms.Registration;
using Geta.GenericLinks.Extensions;

namespace Geta.GenericLinks.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    internal class GenericLinkInitializationModule : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddGenericLinkItems();
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
