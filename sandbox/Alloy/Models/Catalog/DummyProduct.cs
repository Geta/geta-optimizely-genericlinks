using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;

namespace AlloyMvcTemplates.Models.Catalog
{
    [CatalogContentType(
        DisplayName = "DummyProduct",
        GUID = "4965d65c-9415-43dd-ad9c-3d4d080fd27d",
        Description = "")]
    public class DummyProduct : ProductContent
    { }
}
