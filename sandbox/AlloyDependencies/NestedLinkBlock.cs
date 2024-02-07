using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using Geta.Optimizely.GenericLinks;

namespace AlloyDependencies
{
    [ContentType(GUID = "a523fc25-d6a4-4099-aaa2-10b182a15f73")]
    public class NestedLinkBlock : BlockData
    {
        [CultureSpecific]
        public virtual string Name { get; set; }

        [CultureSpecific]
        [Display(Name = "Thumbnail link collection", GroupName = SystemTabNames.Content, Order = 340)]
        public virtual LinkDataCollection<ThumbnailLinkData> ThumbnailLinks { get; set; }
    }
}
