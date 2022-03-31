using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;

namespace Geta.GenericLinks.Cms.Registration
{
    public class LinkDataBackingTypeResolverInterceptor : IBackingTypeResolver
    {
        private readonly IBackingTypeResolver _interceptedResolver;
        private readonly IPropertyDefinitionTypeRepository _propertyDefinitionRepository;
        private readonly IDictionary<Type, Type> _resolvedTypes;
        private readonly Type _collectionBaseType;
        private readonly Type _propertyBaseType;        

        public LinkDataBackingTypeResolverInterceptor(IBackingTypeResolver interceptedResolver, IPropertyDefinitionTypeRepository propertyDefinitionRepository)
        {
            _interceptedResolver = interceptedResolver;
            _propertyDefinitionRepository = propertyDefinitionRepository;
            _collectionBaseType = typeof(LinkDataCollection<>);
            _propertyBaseType = typeof(PropertyLinkDataCollection<>);
            _resolvedTypes = new Dictionary<Type, Type>();
        }

        public virtual Type Resolve(Type type)
        {
            if (_collectionBaseType.IsAssignableFrom(type))
            {
                var resolvedType = TryResolveType(type);
                if (resolvedType is not null)
                    return resolvedType;
            }

            return _interceptedResolver.Resolve(type);
        }

        protected virtual Type? TryResolveType(Type type)
        {
            if (_resolvedTypes.TryGetValue(type, out var resolvedType))
                return resolvedType;

            var linkDataType = type.GenericTypeArguments[0];
            var propertyType = _propertyBaseType.MakeGenericType(linkDataType);

            var definitions = _propertyDefinitionRepository.List();

            foreach (var definition in definitions)
            {
                if (!propertyType.IsAssignableFrom(definition.DefinitionType))
                    continue;

                _resolvedTypes.Add(type, definition.DefinitionType);

                return definition.DefinitionType;
            }

            return null;
        }
    }
}
