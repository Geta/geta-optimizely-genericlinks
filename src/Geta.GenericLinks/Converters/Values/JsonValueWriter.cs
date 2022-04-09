using System;
using System.Text.Json;

namespace Geta.GenericLinks.Converters.Values
{
    public class JsonValueWriter : ILinkDataValueWriter
    {
        public bool CanWrite(Type type)
        {
            if (type.IsAbstract)
                return false;

            if (type.IsInterface)
                return false;

            return typeof(object).IsAssignableFrom(type);
        }

        public void Write(Utf8JsonWriter writer, object value)
        {
            JsonSerializer.Serialize(writer, value.GetType());
        }
    }
}
