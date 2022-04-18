// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Geta.Optimizely.GenericLinks.Tests.Models
{
    public class TestThumbnailLinkData : LinkData
    {
        [Display(Name = "Thumbnail image", Order = 0)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference? Thumbnail
        {
            get => GetAttribute((v) => ContentReference.Parse(v));
            set => SetAttribute(value, (v) => v?.ToString()
                ?? throw new InvalidOperationException("value cannot be null"));
        }

        [Display(Name = "Thumbnail modified", Order = 1000)]
        [UIHint(UIHint.Image)]
        public virtual DateTime? ThumbnailModified
        {
            get => GetAttribute((v) => DateTime.Parse(v, CultureInfo.InvariantCulture));
            set => SetAttribute(value, (v) => v?.ToString(CultureInfo.InvariantCulture) 
                ?? throw new InvalidOperationException("value cannot be null"));
        }

        [Display(Name = "Thumbnail width", Order = 150)]
        [UIHint(UIHint.Image)]
        public virtual int? ThumbnailWidth
        {
            get => GetAttribute((v) => int.Parse(v, CultureInfo.InvariantCulture));
            set => SetAttribute(value, (v) => v?.ToString(CultureInfo.InvariantCulture)
                ?? throw new InvalidOperationException("value cannot be null"));
        }

        [Display(Name = "Thumbnail height", Order = 250)]
        [UIHint(UIHint.Image)]
        public virtual int? ThumbnailHeight
        {
            get => GetAttribute((v) => int.Parse(v, CultureInfo.InvariantCulture));
            set => SetAttribute(value, (v) => v?.ToString(CultureInfo.InvariantCulture)
                ?? throw new InvalidOperationException("value cannot be null"));
        }

        [Display(Name = "Thumbnail aspect", Order = 251)]
        [UIHint(UIHint.Image)]
        public virtual double? ThumbnailAspect
        {
            get => GetAttribute((v) => double.Parse(v, CultureInfo.InvariantCulture));
            set => SetAttribute(value, (v) => v?.ToString(CultureInfo.InvariantCulture)
                ?? throw new InvalidOperationException("value cannot be null"));
        }

        [Display(Name = "Thumbnail tolerance", Order = 252)]
        [UIHint(UIHint.Image)]
        public virtual decimal? ThumbnailTolerance
        {
            get => GetAttribute((v) => decimal.Parse(v, CultureInfo.InvariantCulture));
            set => SetAttribute(value, (v) => v?.ToString(CultureInfo.InvariantCulture)
                ?? throw new InvalidOperationException("value cannot be null"));
        }

        [Display(Name = "Thumbnail caption", Order = 350)]
        public virtual string? ThumbnailCaption
        {
            get => GetAttribute();
            set => SetAttribute(value);
        }
    }
}
