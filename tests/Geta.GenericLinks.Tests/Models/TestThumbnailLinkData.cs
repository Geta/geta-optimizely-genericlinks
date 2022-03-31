using EPiServer.Core;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Geta.GenericLinks.Tests.Models
{
    public class TestThumbnailLinkData : LinkData
    {
        [Display(Name = "Thumbnail image 1", Order = 0)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference? ThumbnailOne
        {
            get
            {
                var attribute = GetAttribute();

                if (ContentReference.TryParse(attribute, out var reference))
                    return reference;

                return null;
            }
            set
            {
                SetAttribute(value);
            }
        }

        [Display(Name = "Thumbnail image 2", Order = 210)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference? ThumbnailTwo
        {
            get
            {
                var attribute = GetAttribute();

                if (ContentReference.TryParse(attribute, out var reference))
                    return reference;

                return null;
            }
            set
            {
                SetAttribute(value);
            }
        }

        [Display(Name = "Thumbnail image 3", Order = 1000)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference? ThumbnailThree
        {
            get
            {
                var attribute = GetAttribute();

                if (ContentReference.TryParse(attribute, out var reference))
                    return reference;

                return null;
            }
            set
            {
                SetAttribute(value);
            }
        }

        protected virtual void SetAttribute(ContentReference? value, [CallerMemberName] string? key = null)
        {
            if (key is null)
                return;

            if (value is null)
            {
                SetAttribute((string?)null, key);
            }
            else
            {
                SetAttribute(value.ToString(), key);
            }
        }

        public override object Clone()
        {
            var item = new TestThumbnailLinkData
            {
                Text = Text,
            };

            item.SetAttributes(Attributes);

            return item;
        }
    }
}