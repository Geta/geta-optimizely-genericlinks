using EPiServer.DataAnnotations;
using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;
using Geta.GenericLinks.Cms.EditorModels;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Geta.GenericLinks.Cms.Metadata
{
    public class DefaultLinkModelMetadataProvider : ILinkModelMetadataProvider
    {
        private readonly ExtensibleMetadataProvider _extensibleMetadataProvider;
        private readonly LocalizationService _localizationService;
        private readonly MetadataHandlerRegistry _metadataHandlerRegistry;
        private readonly ICompositeMetadataDetailsProvider _compositeMetadataDetailsProvider;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;
        private readonly IPropertyReflector _propertyReflector;

        public DefaultLinkModelMetadataProvider(
            ExtensibleMetadataProvider extensibleMetadataProvider,
            LocalizationService localizationService,
            MetadataHandlerRegistry metadataHandlerRegistry,
            ICompositeMetadataDetailsProvider compositeMetadataDetailsProvider,
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IPropertyReflector propertyReflector)
        {
            _extensibleMetadataProvider = extensibleMetadataProvider;
            _localizationService = localizationService;
            _metadataHandlerRegistry = metadataHandlerRegistry;
            _compositeMetadataDetailsProvider = compositeMetadataDetailsProvider;
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider;
            _propertyReflector = propertyReflector;
        }

        public virtual ExtendedMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object>? modelAccessor, Type modelType, string propertyName)
        {
            var defaultProvider = _extensibleMetadataProvider.DefaultProvider;

            DefaultModelMetadata? defaultMetadata;

            if (string.IsNullOrEmpty(propertyName))
            {
                defaultMetadata = (DefaultModelMetadata)defaultProvider.GetMetadataForType(modelType);
            }
            else
            {
                var property = containerType.GetProperty(propertyName);
                if (property is null)
                {
                    var modelIdentity = ModelMetadataIdentity.ForType(modelType);
                    defaultMetadata = GetDefaultMetadataForNonExistingProperty(modelIdentity, modelAccessor);
                }
                else
                {
                    defaultMetadata = (DefaultModelMetadata)defaultProvider.GetMetadataForProperty(containerType, propertyName);
                }
            }

            var metadata = new LinkDataMetadata(defaultMetadata, _validationAttributeAdapterProvider, _extensibleMetadataProvider, containerType, modelAccessor, _localizationService)
            {
                LayoutClass = "epi/shell/layout/ParentContainer",
            };

            ExtractSettingsFromAttributes(metadata, attributes);

            return metadata;
        }

        public virtual IEnumerable<ExtendedMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            if (!typeof(LinkModel).IsAssignableFrom(containerType))
                return Enumerable.Empty<ExtendedMetadata>();

            var properties = _propertyReflector.GetProperties(containerType);
            var baseProperties = new HashSet<string>();
            var extentedMetadata = new List<ExtendedMetadata>();

            foreach (var property in properties)
            {
                var metadata = CreateMetadataForProperty(containerType, property);
                if (metadata is null)
                    continue;

                baseProperties.Add(property.Name);
                extentedMetadata.Add(metadata);
            }

            var modelType = containerType.GetGenericArguments()[0];
            properties = _propertyReflector.GetProperties(modelType, inherited: false);

            foreach (var property in properties)
            {
                if (baseProperties.Contains(property.Name))
                    continue;

                var metadata = CreateMetadataForProperty(modelType, property);
                if (metadata is null)
                    continue;

                extentedMetadata.Add(metadata);
            }

            extentedMetadata.Sort((x, y) => x.Order.CompareTo(y.Order));

            return extentedMetadata;
        }

        protected virtual void ExtractSettingsFromAttributes(LinkDataMetadata metadata, IEnumerable<Attribute> attributes)
        {
            metadata.InitializeFromAttributes(attributes);
            TryAddValidationAttribute<RequiredAttribute>(metadata, attributes);
            TryAddValidationAttribute<RegularExpressionAttribute>(metadata, attributes);
            TryAddValidationAttribute<RangeAttribute>(metadata, attributes);
            TryAddValidationAttribute<StringLengthAttribute>(metadata, attributes);
        }

        protected virtual bool TryAddValidationAttribute<TAttribute>(LinkDataMetadata metadata, IEnumerable<Attribute> attributes)
            where TAttribute : ValidationAttribute
        {
            var validationAttribute = attributes.OfType<TAttribute>().SingleOrDefault();
            if (validationAttribute == null)
                return false;

            TryAddValidationAttribute(metadata, validationAttribute);
            return true;
        }

        protected virtual void TryAddValidationAttribute<TAttribute>(LinkDataMetadata metadata, TAttribute validationAttribute) where TAttribute : ValidationAttribute
        {
            if (!string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                validationAttribute.ErrorMessage = _localizationService.GetString(validationAttribute.ErrorMessage, validationAttribute.ErrorMessage);

            var attributeAdapter = _validationAttributeAdapterProvider.GetAttributeAdapter(validationAttribute, null);
            metadata.AddValidator(attributeAdapter);
        }

        protected virtual DefaultModelMetadata GetDefaultMetadataForNonExistingProperty(ModelMetadataIdentity modelIdentity, Func<object>? modelAccessor)
        {
            return new DefaultModelMetadata(_extensibleMetadataProvider, _compositeMetadataDetailsProvider, new DefaultMetadataDetails(modelIdentity, ModelAttributes.GetAttributesForType(modelIdentity.ModelType))
            {
                BindingMetadata = new BindingMetadata(),
                ValidationMetadata = new ValidationMetadata()
            });
        }

        protected virtual ExtendedMetadata? CreateMetadataForProperty(Type containerType, PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<Attribute>(true);
            var ignoreAttribute = attributes.OfType<IgnoreAttribute>().FirstOrDefault();

            if (ignoreAttribute is not null)
                return null;

            var metadata = CreateMetadata(attributes, containerType, null, property.PropertyType, property.Name);

            IEnumerable<IMetadataHandler> metadataHandlers;

            if (string.IsNullOrEmpty(metadata.TemplateHint))
            {
                metadataHandlers = _metadataHandlerRegistry.GetMetadataHandlers(property.PropertyType);
            }
            else
            {
                metadataHandlers = _metadataHandlerRegistry.GetMetadataHandlers(property.PropertyType, metadata.TemplateHint);
            }

            var extenders = metadataHandlers.OfType<IMetadataExtender>();

            foreach (var extender in extenders)
            {
                extender.ModifyMetadata(metadata, attributes);
            }

            return metadata;
        }
    }
}
