using System.ComponentModel.DataAnnotations;
using EPiServer.Web;
using EPiServer.Core;
using Microsoft.AspNetCore.Html;
using AlloyMvcTemplates.Models.Pages;

namespace AlloyMvcTemplates.Models.ViewModels
{
    public class ContactBlockModel
    {
        [UIHint(UIHint.Image)]
        public ContentReference Image { get; set; }
        public string Heading { get; set; }
        public string LinkText { get; set; }
        public IHtmlContent LinkUrl { get; set; }
        public bool ShowLink { get; set; }
        public ContactPage ContactPage { get; set; }
    }
}