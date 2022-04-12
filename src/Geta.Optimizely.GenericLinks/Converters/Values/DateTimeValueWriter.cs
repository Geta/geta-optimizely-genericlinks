using System;
using System.Text.Json;

namespace Geta.Optimizely.GenericLinks.Converters.Values
{
    public class DateTimeValueWriter : ILinkDataValueWriter
    {
        public bool CanWrite(Type type)
        {
            if (typeof(DateTime).IsAssignableFrom(type))
                return true;

            if (typeof(DateTime?).IsAssignableFrom(type))
                return true;

            return false;
        }

        public void Write(Utf8JsonWriter writer, object value)
        {
            var dateValue = (DateTime)value;
            writer.WriteStringValue(dateValue);
        }
    }
}
