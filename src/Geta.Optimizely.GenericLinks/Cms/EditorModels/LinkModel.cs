// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Geta.Optimizely.GenericLinks.Cms.EditorModels
{
    public class LinkModel
    {
        [Required]
        [DisplayName("/episerver/cms/widget/editlink/linkname")]
        public virtual string? Text { get; set; }

        [DisplayName("/episerver/cms/widget/editlink/linktitle")]
        public virtual string? Title { get; set; }

        [UIHint("TargetFrame")]
        [DisplayName("/contenttypes/icontentdata/properties/pagetargetframe/caption")]
        public virtual int? Target { get; set; }

        [UIHint("HyperLink")]
        public virtual string? Href { get; set; }

        [ScaffoldColumn(false)]
        public virtual string? PublicUrl { get; set; }

        [ScaffoldColumn(false)]
        public virtual string? TypeIdentifier { get; set; }

        [ScaffoldColumn(false)]
        public virtual Dictionary<string, object?> Attributes { get; set; } = new Dictionary<string, object?>();
    }

    public class LinkModel<TLinkData> : LinkModel
        where TLinkData : ILinkData
    {
    }
}
