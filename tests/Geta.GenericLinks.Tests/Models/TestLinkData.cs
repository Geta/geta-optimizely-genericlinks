using Geta.GenericLinks;

namespace Geta.GenericLinks.Tests.Models
{
    public class TestLinkData : LinkData
    {
        public override object Clone()
        {
            var item = new TestLinkData
            {
                Text = Text,
            };

            item.SetAttributes(Attributes);

            return item;
        }
    }
}