using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.Shell;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Geta.GenericLinks;
using Geta.GenericLinks.Cms.EditorModels;
using Geta.GenericLinks.Cms.Metadata;
using Geta.GenericLinks.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Geta.GenericLinks.Converters
{
    public class LinkDataConverter : JsonConverter
    {
        private readonly IPropertyReflector _propertyReflector;
        private readonly IUrlResolver _urlResolver;
        private readonly IFrameRepository _frameRepository;
        private readonly IVirtualPathResolver _virtualPathResolver;
        private readonly UIDescriptorRegistry _uiDescriptors;

        public LinkDataConverter(IPropertyReflector propertyReflector, IUrlResolver urlResolver, IFrameRepository frameRepository, IVirtualPathResolver virtualPathResolver, UIDescriptorRegistry uiDescriptors)
        {
            _propertyReflector = propertyReflector;
            _urlResolver = urlResolver;
            _frameRepository = frameRepository;
            _virtualPathResolver = virtualPathResolver;
            _uiDescriptors = uiDescriptors;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ILinkData).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var clientModel = serializer.Deserialize<LinkModel>(reader);
            if (clientModel is null)
                return null;

            return ToServerModel(objectType, clientModel);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
                return;

            var valueType = value.GetType();
            var clientModel = ToClientModel((ILinkData)value);
            var editorProperties = _propertyReflector.GetProperties(valueType, inherited: false);

            writer.WriteStartObject();

            writer.WritePropertyName("text");
            writer.WriteValue(clientModel.Text);

            writer.WritePropertyName("href");
            writer.WriteValue(clientModel.Href);

            writer.WritePropertyName("title");
            writer.WriteValue(clientModel.Title);

            writer.WritePropertyName("target");
            writer.WriteValue(clientModel.Target);

            writer.WritePropertyName("typeIdentifier");
            writer.WriteValue(clientModel.TypeIdentifier);

            writer.WritePropertyName("publicUrl");
            writer.WriteValue(clientModel.PublicUrl);

            foreach (var property in editorProperties)
            {
                var propertyValue = _propertyReflector.GetValue(valueType, property, value);
                if (propertyValue is null)
                    continue;

                writer.WritePropertyName(property.Name.ToCamel());
                writer.WriteValue(propertyValue.ToString());
            }

            writer.WritePropertyName("attributes");
            writer.WriteStartObject();

            foreach (var attribute in clientModel.Attributes)
            {
                writer.WritePropertyName(attribute.Key);
                writer.WriteValue(attribute.Value);
            }

            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        public virtual LinkModel ToClientModel(ILinkData linkItem)
        {
            var frame = _frameRepository.Load(linkItem.Target);
            var target = frame is not null ? new int?(frame.ID) : null;

            var linkModel = new LinkModel
            {
                Text = linkItem.Text,
                Title = linkItem.Title,
                Target = target,
                Href = _urlResolver.GetPermanent(linkItem.Href, true),
                TypeIdentifier = GetServerTypeIdentifier(),
                Attributes = new Dictionary<string, string>(linkItem.GetAttributes())
            };

            ModifyIContentProperties(linkItem, linkModel);
            return linkModel;
        }

        public virtual ILinkData ToServerModel(Type serverType, LinkModel clientModel)
        {
            if (Activator.CreateInstance(serverType) is not ILinkData linkItem)
                throw new InvalidOperationException("Invalid data type used in conversion, must be assignable to ILinkData");

            if (clientModel.Attributes is not null)
            {
                linkItem.SetAttributes(clientModel.Attributes);
            }

            linkItem.Text = clientModel.Text;
            linkItem.Title = clientModel.Title;
            linkItem.Href = clientModel.Href;

            string target;

            if (clientModel.Target.HasValue)
            {
                var frame = _frameRepository.Load(clientModel.Target.Value);
                target = frame.Name;
            }
            else
            {
                target = string.Empty;
            }

            linkItem.Target = target;

            return linkItem;
        }

        protected virtual string? GetServerTypeIdentifier()
        {
            return _uiDescriptors.GetTypeIdentifier(GetType());
        }

        protected virtual void ModifyIContentProperties(ILinkData serverModel, LinkModel clientModel)
        {
            var mappedHref = _virtualPathResolver.ToAbsoluteOrSame(serverModel.Href);
            if (!string.IsNullOrEmpty(mappedHref))
            {
                UrlBuilder urlBuilder;

                try
                {
                    urlBuilder = new UrlBuilder(mappedHref);
                }
                catch (UriFormatException)
                {
                    return;
                }

                var content = _urlResolver.Route(urlBuilder, ContextMode.Preview);
                if (content is null)
                    return;

                clientModel.TypeIdentifier = _uiDescriptors.GetTypeIdentifier(content.GetType());
                clientModel.PublicUrl = _urlResolver.GetUrl(content.ContentLink);
            }
        }
    }
}
