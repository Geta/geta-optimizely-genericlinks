using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using Geta.Optimizely.GenericLinks;

namespace AlloyDependencies
{
    [ContentType(
        GUID = "7b3bab84-4c16-4f6c-9198-993b5143920f",
        GroupName = "Specialized")]
    public class PropertyThumbnailTestBlock : BlockData
    {
        [CultureSpecific]
        [Display(Name = "Thumbnail link collection", GroupName = SystemTabNames.Content, Order = 10)]
        public virtual LinkDataCollection<ThumbnailLinkData> ThumbnailLinks { get; set; }

        [CultureSpecific]
        [Display(Name = "Single thumbnail link", GroupName = SystemTabNames.Content, Order = 20)]
        public virtual ThumbnailLinkData ThumbnailLink { get; set; }

        [Required]
        [CultureSpecific]
        [Display(Name = "Required thumbnail link", GroupName = SystemTabNames.Content, Order = 30)]
        public virtual ThumbnailLinkData RequiredThumbnailLink { get; set; }
    }
}
