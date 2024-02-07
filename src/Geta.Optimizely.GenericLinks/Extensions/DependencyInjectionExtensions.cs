using EPiServer.Core.Transfer;
using Geta.Optimizely.GenericLinks;
using Geta.Optimizely.GenericLinks.Transfer;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static void AddLinkDataExportTransform<TLinkData>(this IServiceCollection services)
        where TLinkData : LinkData, new()
    {
        services.AddSingleton<PropertyLinkDataCollectionTransform<TLinkData>>().Forward<PropertyLinkDataCollectionTransform<TLinkData>, IPropertyTransform>();
        services.AddSingleton<PropertyLinkDataTransform<TLinkData>>().Forward<PropertyLinkDataTransform<TLinkData>, IPropertyTransform>();
    }
}
