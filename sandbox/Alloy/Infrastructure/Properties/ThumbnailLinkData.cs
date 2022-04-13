using EPiServer.Core;
using Geta.Optimizely.GenericLinks;
using System.ComponentModel.DataAnnotations;

namespace AlloyMvcTemplates.Infrastructure.Properties
{
    public class ThumbnailLinkData : LinkData
    {
        [Display(Name = "Thumbnail image", Order = 0)]
        [UIHint("image")]
        public virtual ContentReference Thumbnail
        {
            get => GetAttribute((v) => ContentReference.Parse(v));
            set => SetAttribute(value, (v) => v.ToString());
        }
    }
}
