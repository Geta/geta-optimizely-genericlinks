using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class TestModelMetadataProvider : IModelMetadataProvider
    {
        private readonly ICompositeMetadataDetailsProvider _compositeMetadataDetailsProvider;
        private readonly IPropertyReflector _propertyReflector;

        public TestModelMetadataProvider(ICompositeMetadataDetailsProvider compositeMetadataDetailsProvider, IPropertyReflector propertyReflector)
        {
            _compositeMetadataDetailsProvider = compositeMetadataDetailsProvider;
            _propertyReflector = propertyReflector;
        }

        public IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType)
        {
            var properties = _propertyReflector.GetProperties(modelType);

            foreach (var property in properties)
            {
                yield return GetMetadataForProperty(modelType, property);
            }
        }

        public virtual ModelMetadata GetMetadataForType(Type modelType)
        {
            var identity = ModelMetadataIdentity.ForType(modelType);
            var attributes = ModelAttributes.GetAttributesForType(modelType);

            var defaultDetails = new DefaultMetadataDetails(identity, attributes);

            return new DefaultModelMetadata(this, _compositeMetadataDetailsProvider, defaultDetails);
        }

        protected virtual ModelMetadata GetMetadataForProperty(Type containerType, PropertyInfo propertyInfo)
        {
            var identity = ModelMetadataIdentity.ForProperty(propertyInfo, propertyInfo.PropertyType, containerType);
            var attributes = ModelAttributes.GetAttributesForProperty(containerType, propertyInfo, propertyInfo.PropertyType);

            var defaultDetails = new DefaultMetadataDetails(identity, attributes);

            return new DefaultModelMetadata(this, _compositeMetadataDetailsProvider, defaultDetails);
        }
    }
}
