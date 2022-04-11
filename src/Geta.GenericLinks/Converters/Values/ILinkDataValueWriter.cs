using System;
using System.Text.Json;

namespace Geta.GenericLinks.Converters.Values
{
    public interface ILinkDataValueWriter
    {
        bool CanWrite(Type type);
        void Write(Utf8JsonWriter writer, object value);
    }
}
