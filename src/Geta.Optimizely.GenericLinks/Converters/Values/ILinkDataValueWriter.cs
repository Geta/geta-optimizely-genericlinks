// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Text.Json;

namespace Geta.Optimizely.GenericLinks.Converters.Values
{
    public interface ILinkDataValueWriter
    {
        bool CanWrite(Type type);
        void Write(Utf8JsonWriter writer, object value);
    }
}
