using EPiServer.Core;
using EPiServer.DataAbstraction;
using Geta.GenericLinks;
using System;
using System.Collections.Generic;

namespace Geta.GenericLinks.Cms.Registration
{
    public class PropertyLinkDataDefinitionsLoader
    {
        private readonly IPropertyDefinitionTypeRepository _propertyDefinitionTypeRepository;

        public PropertyLinkDataDefinitionsLoader(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository)
        {
            _propertyDefinitionTypeRepository = propertyDefinitionTypeRepository;
        }

        public IEnumerable<Type> Load()
        {
            var propertyDefinitions = _propertyDefinitionTypeRepository.List();

            foreach (var propertyDefinition in propertyDefinitions)
            {
                if (propertyDefinition.IsSystemType())
                    continue;

                if (string.IsNullOrEmpty(propertyDefinition.TypeName))
                    continue;

                var definitionType = propertyDefinition.DefinitionType;

                if (!typeof(PropertyLinkDataCollection).IsAssignableFrom(definitionType))
                    continue;

                var genericType = FindBaseGenericType(definitionType);
                if (genericType is null)
                    continue;

                yield return genericType;
            }
        }

        private Type? FindBaseGenericType(Type definitionType)
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

                    if (type == typeof(PropertyLinkDataCollection))
                        return null;
                }
                while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(PropertyLinkDataCollection<>));
                genericArgument = type.GetGenericArguments()[0];
            }
            while (genericArgument.IsValueType || genericArgument == typeof(ContentReference) || genericArgument == typeof(string));
            return genericArgument;
        }
    }
}
