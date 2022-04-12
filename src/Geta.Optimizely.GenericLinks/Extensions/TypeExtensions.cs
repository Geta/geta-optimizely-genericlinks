using EPiServer.Core;
using System;

namespace Geta.Optimizely.GenericLinks.Extensions
{
    internal static class TypeExtensions
    {
        public static Type? FindBaseGenericType(this Type definitionType, Type baseType)
        {
            Type? type = definitionType;
            Type genericArgument;
            do
            {
                do
                {
                    type = type.BaseType;

                    if (type is null)
                        return null;

                    if (type == baseType)
                        return null;
                }
                while (!type.IsGenericType);
                genericArgument = type.GetGenericArguments()[0];
            }
            while (genericArgument.IsValueType || genericArgument == typeof(ContentReference) || genericArgument == typeof(string));
            return genericArgument;
        }
    }
}
