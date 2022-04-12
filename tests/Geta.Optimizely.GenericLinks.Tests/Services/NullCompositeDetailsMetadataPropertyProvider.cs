// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Geta.Optimizely.GenericLinks.Tests.Services
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
