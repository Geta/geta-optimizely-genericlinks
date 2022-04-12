// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class FakeValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
        {
            return new FakeAttributeAdapter();
        }
    }
}
