using System;

namespace Geta.GenericLinks.Converters.Attributes
{
    public interface IAttributeConverter
    {
        bool CanConvert(Type type);
        string? Convert(object value);
    }
}
