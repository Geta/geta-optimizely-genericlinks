using EPiServer.Core;
using EPiServer.DataAbstraction;
using System;

namespace Geta.Optimizely.GenericLinks.Tests.Extensions
{
    public static class TypeExtensions
    {
        public static PropertyDefinitionType ToDefinition(this Type type, int id, PropertyDataType dataType)
        {
            var propertyTypeName = type.FullName;
            var propertyAssemblyName = type.Assembly.FullName;

            return new PropertyDefinitionType(id, dataType, type.Name, propertyTypeName, propertyAssemblyName);
        }
    }
}
