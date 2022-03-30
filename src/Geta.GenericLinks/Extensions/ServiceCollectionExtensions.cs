using Geta.GenericLinks.Cms.Metadata;
using Geta.GenericLinks.Cms.Registration;
using Geta.GenericLinks.Converters;
using Geta.GenericLinks.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace Geta.GenericLinks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGenericLinkItems(this IServiceCollection services)
        {
            services.AddSingleton<PropertyLinkDataDefinitionsLoader>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<JsonConverter, LinkDataConverter>());
            services.TryAddTransient<ILinkModelMetadataProvider, DefaultLinkModelMetadataProvider>();
            services.TryAddSingleton<IPropertyReflector, DefaultPropertyReflector>();
            services.TryAddTransient<IAttributeSanitizer, DefaultAttributeSanitizer>();
            services.TryAddTransient<ILinkHtmlSerializer, DefaultLinkHtmlSerializer>();
        }
    }
}
