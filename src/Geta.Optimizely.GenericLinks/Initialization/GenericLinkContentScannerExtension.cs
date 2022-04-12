// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using Geta.Optimizely.GenericLinks.Extensions;
using System;

namespace Geta.Optimizely.GenericLinks.Initialization
{
    public class GenericLinkContentScannerExtension : ContentScannerExtension
    {
        private readonly IPropertyDefinitionTypeRepository _propertyDefinitionTypeRepository;

        public GenericLinkContentScannerExtension(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository)
        {
            _propertyDefinitionTypeRepository = propertyDefinitionTypeRepository;
        }

        public override void AssignValuesToProperties(ContentTypeModel contentTypeModel)
        {
            foreach (var propertyDefinition in contentTypeModel.PropertyDefinitionModels)
            {
                if (typeof(LinkDataCollection).IsAssignableFrom(propertyDefinition.Type))
                {
                    TryAssignBackingtype(propertyDefinition, propertyDefinition.Type, typeof(PropertyLinkDataCollection));
                }
                else if (typeof(LinkData).IsAssignableFrom(propertyDefinition.Type))
                {
                    TryAssignBackingtype(propertyDefinition, propertyDefinition.Type, typeof(PropertyLinkData));
                }
            }
        }

        protected virtual void TryAssignBackingtype(PropertyDefinitionModel propertyDefinition, Type propertyType, Type backingType)
        {
            if (propertyDefinition.BackingType is not null)
                return;

            if (propertyDefinition.ExistingPropertyDefinition is not null)
                return;

            propertyDefinition.BackingType = GetBackingType(propertyType, backingType);
        }

        protected virtual Type? GetBackingType(Type propertyType, Type backingType)
        {
            var baseType = propertyType.GenericTypeArguments.Length > 0 ? propertyType.GenericTypeArguments[0] : propertyType;
            var definitions = _propertyDefinitionTypeRepository.List();

            foreach (var definition in definitions)
            {
                if (!IsQualified(definition, backingType))
                    continue;

                var genericType = definition.DefinitionType.FindBaseGenericType(backingType);
                if (baseType != genericType)
                    continue;

                return definition.DefinitionType;
            }

            return null;
        }

        protected virtual bool IsQualified(PropertyDefinitionType propertyDefinition, Type backingType)
        {
            if (propertyDefinition.IsSystemType())
                return false;

            if (string.IsNullOrEmpty(propertyDefinition.TypeName))
                return false;

            if (!backingType.IsAssignableFrom(propertyDefinition.DefinitionType))
                return false;

            return true;
        }
    }
}
