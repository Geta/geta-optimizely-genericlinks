// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using System;
using System.Text.Json;

namespace Geta.Optimizely.GenericLinks.Converters.Values
{
    public class ContentReferenceValueWriter : ILinkDataValueWriter
    {
        public bool CanWrite(Type type)
        {
            return typeof(ContentReference).IsAssignableFrom(type);
        }

        public void Write(Utf8JsonWriter writer, object value)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
