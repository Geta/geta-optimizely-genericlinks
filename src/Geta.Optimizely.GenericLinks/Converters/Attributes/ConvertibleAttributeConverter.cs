using System;
using System.Globalization;

namespace Geta.Optimizely.GenericLinks.Converters.Attributes
{
    public class ConvertibleAttributeConverter : ILinkDataAttibuteConverter
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
