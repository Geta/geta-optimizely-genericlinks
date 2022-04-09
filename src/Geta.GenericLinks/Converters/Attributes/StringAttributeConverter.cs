using System;

namespace Geta.GenericLinks.Converters.Attributes
{
    public class StringAttributeConverter : IAttributeConverter
    {
        public bool CanConvert(Type type)
        {
            return typeof(string).IsAssignableFrom(type);
        }

        public string? Convert(object value)
        {
            return (string)value;
        }
    }
}
