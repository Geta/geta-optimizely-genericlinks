using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace Geta.GenericLinks.Tests.Services
{
    public class NullCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
        }
    }
}
