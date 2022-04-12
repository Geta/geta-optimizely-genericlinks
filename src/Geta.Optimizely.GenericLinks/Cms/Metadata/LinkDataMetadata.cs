// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;
using Geta.Optimizely.GenericLinks.Extensions;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Cms.Metadata
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

        public virtual void ExtendFromAttributes(IEnumerable<Attribute> attributes)
        {
            ReadSettingsFromUIHintAttributes(attributes.OfType<UIHintAttribute>());
            ReadSettingsFromEditableAttributes(attributes);
            ReadSettingsFromScaffoldColumnAttribute(attributes.FirstOfType<ScaffoldColumnAttribute>());
            ReadSettingsFromDisplayAttributes(attributes);
            ReadSettingsFromRequiredAttribute(attributes.FirstOfType<RequiredAttribute>());
            ReadSettingsFromGroupSettingsAttribute(attributes.FirstOfType<GroupSettingsAttribute>());
            ReadSettingsFromHintAttributes(attributes);
        }

    }
}
