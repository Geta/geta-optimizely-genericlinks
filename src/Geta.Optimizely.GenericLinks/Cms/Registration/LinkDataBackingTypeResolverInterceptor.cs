// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Cms.Registration
{
    public class LinkDataBackingTypeResolverInterceptor : IBackingTypeResolver
    {
        private readonly IBackingTypeResolver _interceptedResolver;
        private readonly IPropertyDefinitionTypeRepository _propertyDefinitionRepository;
        private readonly IDictionary<Type, Type> _resolvedTypes;
        private readonly Type _baseType;
        private readonly Type _collectionBaseType;
        private readonly Type _propertyBaseType;
        private readonly Type _collectionPropertyBaseType;

        public LinkDataBackingTypeResolverInterceptor(IBackingTypeResolver interceptedResolver, IPropertyDefinitionTypeRepository propertyDefinitionRepository)
        {
            _interceptedResolver = interceptedResolver;
            _propertyDefinitionRepository = propertyDefinitionRepository;
            _baseType = typeof(LinkData);
            _collectionBaseType = typeof(LinkDataCollection);
            _propertyBaseType = typeof(PropertyLinkData<>);
            _collectionPropertyBaseType = typeof(PropertyLinkDataCollection<>);
            _resolvedTypes = new Dictionary<Type, Type>();
        }

        public virtual Type Resolve(Type type)
        {
            if (_baseType.IsAssignableFrom(type))
            {
                var resolvedType = TryResolveType(type, _propertyBaseType);
                if (resolvedType is not null)
                    return resolvedType;
            }

            if (_collectionBaseType.IsAssignableFrom(type))
            {
                var resolvedType = TryResolveType(type, _collectionPropertyBaseType);
                if (resolvedType is not null)
                    return resolvedType;
            }

            return _interceptedResolver.Resolve(type);
        }

        protected virtual Type? TryResolveType(Type type, Type baseType)
        {
            if (_resolvedTypes.TryGetValue(type, out var resolvedType))
                return resolvedType;

            var linkDataType = type.GenericTypeArguments.Length > 0 ? type.GenericTypeArguments[0] : type;
            var propertyType = baseType.MakeGenericType(linkDataType);

            var definitions = _propertyDefinitionRepository.List();
            var definitionTypes = definitions.Select(d => d.DefinitionType);

            foreach (var definitionType in definitionTypes)
            {
                if (!propertyType.IsAssignableFrom(definitionType))
                    continue;

                _resolvedTypes.Add(type, definitionType);

                return definitionType;
            }

            return null;
        }
    }
}
