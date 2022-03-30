using EPiServer.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Geta.GenericLinks.Cms.EditorModels
{
    public class LinkModel
    {
        [Required]
        [DisplayName("/episerver/cms/widget/editlink/linkname")]
        [Display(Order = 100)]
        public virtual string? Text { get; set; }

        [DisplayName("/episerver/cms/widget/editlink/linktitle")]
        [Display(Order = 200)]
        public virtual string? Title { get; set; }

        [UIHint("TargetFrame")]
        [Display(Order = 300)]
        [DisplayName("/contenttypes/icontentdata/properties/pagetargetframe/caption")]
        public virtual int? Target { get; set; }

        [Required]
        [UIHint("HyperLink")]
        [Display(Order = 400)]
        public virtual string? Href { get; set; }

        [ScaffoldColumn(false)]
        public virtual string? PublicUrl { get; set; }

        [ScaffoldColumn(false)]
        public virtual string? TypeIdentifier { get; set; }

        [ScaffoldColumn(false)]
        public virtual Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }

    public class LinkModel<TLinkData> : LinkModel
        where TLinkData : ILinkData
    {
    }
}
