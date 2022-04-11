using System;

namespace Geta.GenericLinks.Converters.Attributes
{
    public interface ILinkDataAttibuteConverter
    {
        bool CanConvert(Type type);
        string? Convert(object value);
    }
}
