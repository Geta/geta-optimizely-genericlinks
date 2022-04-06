using EPiServer.DataAbstraction;

namespace Geta.GenericLinks.Cms.Registration
{
    public class PropertyLinkDataDefinitionsLoader : DataDefinitionsLoaderBase
    {
        public PropertyLinkDataDefinitionsLoader(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository) 
            : base(typeof(PropertyLinkData), propertyDefinitionTypeRepository)
        {
            
        }
    }
}
