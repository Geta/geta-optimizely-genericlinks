using EPiServer.PlugIn;
using Geta.Optimizely.GenericLinks;

namespace AlloyDependencies
{
    [PropertyDefinitionTypePlugIn(DisplayName = "Link with thumbnail", GUID = "256acbba-100b-4f60-8e2b-08b399036228")]
    public class PropertyThumbnail : PropertyLinkData<ThumbnailLinkData>
    {

    }
}
