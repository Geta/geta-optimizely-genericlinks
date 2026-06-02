using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using Geta.Optimizely.GenericLinks;

namespace Geta.Optimizely.GenericLinks.Web.Models;

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
