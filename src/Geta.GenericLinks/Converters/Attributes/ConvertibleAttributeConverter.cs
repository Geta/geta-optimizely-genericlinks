using System;
using System.Globalization;

namespace Geta.GenericLinks.Converters.Attributes
{
    public class ConvertibleAttributeConverter : IAttributeConverter
    {
        public bool CanConvert(Type type)
        {
            return typeof(IConvertible).IsAssignableFrom(type);
        }

        public string? Convert(object value)
        {
            if (value is null)
                return null;

            return System.Convert.ToString((IConvertible)value, CultureInfo.InvariantCulture);
        }
    }
}
