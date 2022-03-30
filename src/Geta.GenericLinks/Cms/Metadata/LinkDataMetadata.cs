using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace Geta.GenericLinks.Cms.Metadata
{
    public class LinkDataMetadata : ExtendedMetadata
    {
        public LinkDataMetadata(
            DefaultModelMetadata defaultMetadata,
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            ExtensibleMetadataProvider provider,
            Type? containerType = null,
            Func<object>? modelAccessor = null,
            LocalizationService? localizationService = null)
            : base(defaultMetadata, validationAttributeAdapterProvider, provider, containerType, modelAccessor, localizationService)
        {
        }

        protected override void ReadSettingsFromDataTypeAttribute(DataTypeAttribute? attribute)
        {
        }
    }
}
