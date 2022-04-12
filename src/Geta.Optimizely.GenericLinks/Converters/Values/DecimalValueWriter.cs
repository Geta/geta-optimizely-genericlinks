// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Text.Json;

namespace Geta.Optimizely.GenericLinks.Converters.Values
{
    public class DecimalValueWriter : ILinkDataValueWriter
    {
        public bool CanWrite(Type type)
        {
            if (typeof(decimal).IsAssignableFrom(type))
                return true;

            if (typeof(decimal?).IsAssignableFrom(type))
                return true;

            return false;
        }

        public void Write(Utf8JsonWriter writer, object value)
        {
            writer.WriteNumberValue((decimal)value);
        }
    }
}
