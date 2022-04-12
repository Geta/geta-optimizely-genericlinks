using Geta.Optimizely.GenericLinks;
using System;

namespace Geta.Optimizely.GenericLinks.Html
{
    public interface ILinkHtmlSerializer
    {
        string Serialize<TLinkData>(TLinkData? link, StringMode mode)
            where TLinkData : ILinkData;

        string Serialize<TLinkData>(LinkDataCollection<TLinkData>? links, StringMode mode)
            where TLinkData : ILinkData;
    }
}
