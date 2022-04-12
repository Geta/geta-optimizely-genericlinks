using EPiServer.DataAbstraction;

namespace Geta.Optimizely.GenericLinks.Cms.Registration
{
    public class PropertyLinkDataDefinitionsLoader : DataDefinitionsLoaderBase
    {
        public PropertyLinkDataDefinitionsLoader(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository)
            : base(typeof(PropertyLinkData), propertyDefinitionTypeRepository)
        {

        }
    }
}
