using System;

namespace Geta.GenericLinks.Html
{
    public interface ILinkHtmlSerializer
    {
        string Serialize<TLinkData>(LinkDataCollection<TLinkData>? links, StringMode mode)
            where TLinkData : ILinkData;
    }
}
