using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Geta.Optimizely.GenericLinks.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Geta.Optimizely.GenericLinks.Web.Components;

[TemplateDescriptor]
public class NestedLinkBlockComponent : AsyncBlockComponent<NestedLinkBlock>
{
    protected override async Task<IViewComponentResult> InvokeComponentAsync(NestedLinkBlock currentBlock)
    {
        return await Task.FromResult(View("/Views/NestedLinkBlock.cshtml", currentBlock));
    }
}
