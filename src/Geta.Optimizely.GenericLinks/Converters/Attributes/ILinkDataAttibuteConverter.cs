// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace Geta.Optimizely.GenericLinks.Converters.Attributes;

public interface ILinkDataAttibuteConverter
{
    bool CanConvert(Type type);
    string? Convert(object value);
}
