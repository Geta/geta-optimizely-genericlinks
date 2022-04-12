using EPiServer.DataAbstraction;
using Geta.Optimizely.GenericLinks;

namespace Geta.Optimizely.GenericLinks.Cms.Registration
{
    public class PropertyLinkDataCollectionDefinitionsLoader : DataDefinitionsLoaderBase
    {
        public PropertyLinkDataCollectionDefinitionsLoader(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository)
            : base(typeof(PropertyLinkDataCollection), propertyDefinitionTypeRepository)
        {

        }
    }
}
