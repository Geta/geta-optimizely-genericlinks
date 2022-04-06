using EPiServer.DataAbstraction;

namespace Geta.GenericLinks.Cms.Registration
{
    public class PropertyLinkDataCollectionDefinitionsLoader : DataDefinitionsLoaderBase
    {
        public PropertyLinkDataCollectionDefinitionsLoader(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository) 
            : base(typeof(PropertyLinkDataCollection), propertyDefinitionTypeRepository)
        {
            
        }
    }
}
