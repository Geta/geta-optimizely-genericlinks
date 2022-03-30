using EPiServer.Core;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Geta.GenericLinks
{
    public class ThumbnailLinkData : LinkData
    {
        [Display(Name = "Thumbnail image", Order = 0)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference? Thumbnail
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
            var item = new ThumbnailLinkData
            {
                Text = Text,
            };

            item.SetAttributes(Attributes);

            return item;
        }
    }
}
