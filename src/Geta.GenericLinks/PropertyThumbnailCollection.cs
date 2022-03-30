using EPiServer.PlugIn;

namespace Geta.GenericLinks
{
    [PropertyDefinitionTypePlugIn(DisplayName = "Link collection with thumbnails")]
    public class PropertyThumbnailCollection : PropertyLinkDataCollection<ThumbnailLinkData>
    {

    }
}
