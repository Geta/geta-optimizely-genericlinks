// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAbstraction;
using Geta.Optimizely.GenericLinks.Extensions;
using System;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks.Cms.Registration;

public abstract class DataDefinitionsLoaderBase
{
    private readonly Type _baseType;
    private readonly IPropertyDefinitionTypeRepository _propertyDefinitionTypeRepository;

    protected DataDefinitionsLoaderBase(Type baseType, IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository)
    {
        _baseType = baseType;
        _propertyDefinitionTypeRepository = propertyDefinitionTypeRepository;
    }

    public virtual IEnumerable<Type> Load()
    {
        var propertyDefinitions = _propertyDefinitionTypeRepository.List();

        foreach (var propertyDefinition in propertyDefinitions)
        {
            if (!IsQualified(propertyDefinition))
                continue;

            var genericType = propertyDefinition.DefinitionType.FindBaseGenericType(_baseType);
            if (genericType is null)
                continue;

            yield return genericType;
        }
    }

    protected virtual bool IsQualified(PropertyDefinitionType propertyDefinition)
    {
        if (propertyDefinition.IsSystemType())
            return false;

        if (string.IsNullOrEmpty(propertyDefinition.TypeName))
            return false;

        if (!_baseType.IsAssignableFrom(propertyDefinition.DefinitionType))
            return false;

        return true;
    }
}
