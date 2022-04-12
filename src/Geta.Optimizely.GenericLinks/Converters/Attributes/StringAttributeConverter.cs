using System;

namespace Geta.Optimizely.GenericLinks.Converters.Attributes
{
    public class StringAttributeConverter : ILinkDataAttibuteConverter
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
