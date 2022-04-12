using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class NullValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        [return: MaybeNull]
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            return null;
        }
    }
}
