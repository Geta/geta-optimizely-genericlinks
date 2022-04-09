using Newtonsoft.Json;
using System;

namespace Geta.GenericLinks.Converters.Attributes
{
    public class JsonAttributeConverter : IAttributeConverter
    {
        public bool CanConvert(Type type)
        {
            if (type.IsAbstract)
                return false;

            if (type.IsInterface)
                return false;

            return typeof(object).IsAssignableFrom(type);
        }

        public string? Convert(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
