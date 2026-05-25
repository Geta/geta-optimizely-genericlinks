// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAbstraction;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Cms.Registration;

public class LinkDataBackingTypeResolverInterceptor : IBackingTypeResolver
{
    private readonly IBackingTypeResolver _interceptedResolver;
    private readonly IPropertyDefinitionTypeRepository _propertyDefinitionRepository;
    private readonly ConcurrentDictionary<Type, PropertyDefinitionType> _resolvedTypes;
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
        _resolvedTypes = new ConcurrentDictionary<Type, PropertyDefinitionType>();
    }

    public virtual PropertyDefinitionTypeResolution Resolve(Type type)
    {
        if (_baseType.IsAssignableFrom(type))
        {
            var resolved = TryResolveDefinition(type, _propertyBaseType);
            if (resolved is not null)
                return new PropertyDefinitionTypeResolution(resolved, PropertyDefinitionKind.Value);
        }

        if (_collectionBaseType.IsAssignableFrom(type))
        {
            var resolved = TryResolveDefinition(type, _collectionPropertyBaseType);
            if (resolved is not null)
                return new PropertyDefinitionTypeResolution(resolved, PropertyDefinitionKind.Value);
        }

        return _interceptedResolver.Resolve(type);
    }

    protected virtual PropertyDefinitionType? TryResolveDefinition(Type type, Type baseType)
    {
        if (_resolvedTypes.TryGetValue(type, out var cached))
            return cached;

        var linkDataType = type.GenericTypeArguments.Length > 0 ? type.GenericTypeArguments[0] : type;
        var propertyType = baseType.MakeGenericType(linkDataType);

        var definitions = _propertyDefinitionRepository.List();

        foreach (var definition in definitions)
        {
            if (!propertyType.IsAssignableFrom(definition.DefinitionType))
                continue;

            _resolvedTypes.TryAdd(type, definition);

            return definition;
        }

        return null;
    }
}
