using System;
using System.Text.Json;

namespace Geta.Optimizely.GenericLinks.Converters.Values
{
    public class StringValueWriter : ILinkDataValueWriter
    {
        public bool CanWrite(Type type)
        {
            return typeof(string).IsAssignableFrom(type);
        }

        public void Write(Utf8JsonWriter writer, object value)
        {
            writer.WriteStringValue((string)value);
        }
    }
}
