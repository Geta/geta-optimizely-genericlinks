using System;
using System.Text.Json;

namespace Geta.GenericLinks.Converters.Values
{
    public class DoubleValueWriter : ILinkDataValueWriter
    {
        public bool CanWrite(Type type)
        {
            if (typeof(double).IsAssignableFrom(type))
                return true;

            if (typeof(double?).IsAssignableFrom(type))
                return true;

            return false;
        }

        public void Write(Utf8JsonWriter writer, object value)
        {
            writer.WriteNumberValue((double)value);
        }
    }
}
