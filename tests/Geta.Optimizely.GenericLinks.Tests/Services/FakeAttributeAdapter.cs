using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class FakeAttributeAdapter : IAttributeAdapter
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            
        }

        public string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return string.Empty;
        }
    }
}
