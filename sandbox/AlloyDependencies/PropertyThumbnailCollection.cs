﻿using EPiServer.PlugIn;
using Geta.Optimizely.GenericLinks;

namespace AlloyDependencies
{
    [PropertyDefinitionTypePlugIn(DisplayName = "Link collection with thumbnails", GUID = "9f711ce6-ee75-466c-ab9c-67b65a85abc1")]
    public class PropertyThumbnailCollection : PropertyLinkDataCollection<ThumbnailLinkData>
    {

    }
}
