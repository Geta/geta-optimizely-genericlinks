// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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
