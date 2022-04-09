using EPiServer.Core;
using System;
using System.Text.Json;

namespace Geta.GenericLinks.Converters.Values
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