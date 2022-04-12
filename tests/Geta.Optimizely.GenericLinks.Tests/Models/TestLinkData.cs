using Geta.Optimizely.GenericLinks;

namespace Geta.Optimizely.GenericLinks.Tests.Models
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