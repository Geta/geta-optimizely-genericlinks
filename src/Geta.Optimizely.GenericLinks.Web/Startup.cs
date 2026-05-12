using System.Reflection;
using EPiServer.Framework.Hosting;
using EPiServer.Web.Hosting;
using Geta.Optimizely.GenericLinks.Web.Models;
using Geta.Optimizely.GenericLinks.Web.Services;
using Optimizely.Graph.Cms.Configuration;

namespace Geta.Optimizely.GenericLinks.Web;

public class Startup
{
    private readonly Foundation.Startup _foundationStartup;
    private readonly IConfiguration _configuration;

    public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
    {
        _foundationStartup = new Foundation.Startup(webHostingEnvironment, configuration);
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        _foundationStartup.ConfigureServices(services);

        var graphAppKey = _configuration["Optimizely:ContentGraph:AppKey"];
        if (string.IsNullOrEmpty(graphAppKey))
        {
            var syncClientType = typeof(GraphCmsOptions).Assembly
                .GetType("Optimizely.Graph.Cms.Client.ISyncClient");
            if (syncClientType != null)
            {
                var descriptor = services.FirstOrDefault(d => d.ServiceType == syncClientType);
                if (descriptor != null) services.Remove(descriptor);

                var createMethod = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m => m.Name == nameof(DispatchProxy.Create)
                                && m.IsGenericMethodDefinition
                                && m.GetGenericArguments().Length == 2);
                var proxy = createMethod
                    .MakeGenericMethod(syncClientType, typeof(NoOpSyncClientProxy))
                    .Invoke(null, null)!;

                services.AddSingleton(syncClientType, proxy);
            }
        }

        services.AddLinkDataExportTransform<ThumbnailLinkData>();

        var moduleName = typeof(LinkData).Assembly.GetName().Name;
        var fullPath = Path.GetFullPath($"../{moduleName}");

        services.Configure<CompositeFileProviderOptions>(options =>
        {
            options.BasePathFileProviders.Add(new MappingPhysicalFileProvider(
                                                  $"/Optimizely/{moduleName}",
                                                  string.Empty,
                                                  fullPath));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        _foundationStartup.Configure(app, env);
    }
}
