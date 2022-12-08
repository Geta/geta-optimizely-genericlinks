using EPiServer.Commerce.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using CommerceInitializationModule = EPiServer.Commerce.Initialization.InitializationModule;

namespace AlloyMvcTemplates.Business.Initialization
{

    [ModuleDependency(typeof(CommerceInitializationModule))]
    public class CommerceInitialization : IInitializableModule
    {

        public void Initialize(InitializationEngine context)
        {
            CatalogRouteHelper.MapDefaultHierarchialRouter(false);

            var catalogRoot = context.Locate.Advanced.GetInstance<EnableCatalogRoot>();
            catalogRoot.SetCatalogAccessRights();
        }
        
        public void Uninitialize(InitializationEngine context) { }
    }
}
