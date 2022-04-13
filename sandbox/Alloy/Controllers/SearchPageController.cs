using System.Linq;
using AlloyMvcTemplates.Models.Pages;
using AlloyMvcTemplates.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AlloyMvcTemplates.Controllers
{
    public class SearchPageController : PageControllerBase<SearchPage>
    {
        public ViewResult Index(SearchPage currentPage, string q)
        {
            //TODO: Install NuGet package EPiServer.Find.Cms to add search capabilities
            var model = new SearchContentModel(currentPage)
            {
                Hits = Enumerable.Empty<SearchContentModel.SearchHit>(),
                NumberOfHits = 0,
                SearchServiceDisabled = true,
                SearchedQuery = q
            };

            return View(model);
        }
    }
}
