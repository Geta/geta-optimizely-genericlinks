using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Geta.Optimizely.GenericLinks.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Geta.Optimizely.GenericLinks.Web.Components;

[TemplateDescriptor]
public class PropertyThumbnailTestBlockComponent : AsyncBlockComponent<PropertyThumbnailTestBlock>
{
    protected override Task<IViewComponentResult> InvokeComponentAsync(PropertyThumbnailTestBlock currentBlock)
    {
        return Task.FromResult<IViewComponentResult>(View("/Views/PropertyThumbnailTestBlock.cshtml", currentBlock));
    }
}
