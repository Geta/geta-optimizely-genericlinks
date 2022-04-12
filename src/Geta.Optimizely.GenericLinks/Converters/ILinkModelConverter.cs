using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using System;

namespace Geta.Optimizely.GenericLinks.Converters
{
    public interface ILinkModelConverter
    {
        LinkModel ToClientModel(ILinkData linkItem);
        ILinkData ToServerModel(Type serverType, LinkModel clientModel);
        TLinkData ToServerModel<TLinkData>(LinkModel clientModel)
            where TLinkData : ILinkData, new();
    }
}
