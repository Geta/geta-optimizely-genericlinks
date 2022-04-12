using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.Shell;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using Geta.Optimizely.GenericLinks.Converters.Attributes;
using Geta.Optimizely.GenericLinks.Extensions;
using System;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks.Converters
{
    public class DefaultLinkModelConverter : ILinkModelConverter
    {
        private readonly IUrlResolver _urlResolver;
        private readonly IFrameRepository _frameRepository;
        private readonly IVirtualPathResolver _virtualPathResolver;
        private readonly IEnumerable<ILinkDataAttibuteConverter> _attibuteConverters;
        private readonly IDictionary<Type, ILinkDataAttibuteConverter> _resolvedAttributeConverters;
        private readonly UIDescriptorRegistry _uiDescriptors;

        public DefaultLinkModelConverter(
            IUrlResolver urlResolver,
            IFrameRepository frameRepository,
            IVirtualPathResolver virtualPathResolver,
            IEnumerable<ILinkDataAttibuteConverter> attibuteConverters,
            UIDescriptorRegistry uiDescriptors)
        {
            _urlResolver = urlResolver;
            _frameRepository = frameRepository;
            _virtualPathResolver = virtualPathResolver;
            _attibuteConverters = attibuteConverters;
            _resolvedAttributeConverters = new Dictionary<Type, ILinkDataAttibuteConverter>();
            _uiDescriptors = uiDescriptors;
        }

        public virtual LinkModel ToClientModel(ILinkData linkItem)
        {
            var frame = _frameRepository.Load(linkItem.Target);
            var target = frame is not null ? new int?(frame.ID) : null;
            var attributes = ToClientAttributes(linkItem.GetAttributes());

            var linkModel = new LinkModel
            {
                Text = linkItem.Text,
                Title = linkItem.Title,
                Target = target,
                Href = _urlResolver.GetPermanent(linkItem.Href, true),
                TypeIdentifier = GetServerTypeIdentifier(),
                Attributes = attributes
            };

            ModifyIContentProperties(linkItem, linkModel);
            return linkModel;
        }

        public TLinkData ToServerModel<TLinkData>(LinkModel clientModel) where TLinkData : ILinkData, new()
        {
            var linkItem = new TLinkData();
            PopulateValues(linkItem, clientModel);

            return linkItem;
        }

        public virtual ILinkData ToServerModel(Type serverType, LinkModel clientModel)
        {
            if (Activator.CreateInstance(serverType) is not ILinkData linkItem)
                throw new InvalidOperationException("Invalid data type used in conversion, must be assignable to ILinkData");

            PopulateValues(linkItem, clientModel);

            return linkItem;
        }

        protected virtual void PopulateValues(ILinkData serverModel, LinkModel clientModel)
        {
            if (clientModel.Attributes is not null)
            {
                var clientAttributes = ToServerAttributes(clientModel.Attributes);
                serverModel.SetAttributes(clientAttributes);
            }

            serverModel.Text = clientModel.Text;
            serverModel.Title = clientModel.Title;
            serverModel.Href = clientModel.Href;

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

            serverModel.Target = target;
        }

        protected virtual Dictionary<string, object?> ToClientAttributes(IDictionary<string, string> attributes)
        {
            var clientAttributes = new Dictionary<string, object?>(attributes.Count);

            foreach (var attribute in attributes)
            {
                clientAttributes.Add(attribute.Key, attribute.Value);
            }

            return clientAttributes;
        }

        protected virtual Dictionary<string, string> ToServerAttributes(IDictionary<string, object?> attributes)
        {
            var serverAttributes = new Dictionary<string, string>(attributes.Count);

            foreach (var attribute in attributes)
            {
                var attributeValue = attribute.Value;
                if (attributeValue is null)
                    continue;

                var type = attributeValue.GetType();
                var converter = FindAttributeConverter(type);

                string? value;

                if (converter is null)
                {
                    value = attributeValue.ToString();
                }
                else
                {
                    value = converter.Convert(attributeValue);
                }

                if (value is null)
                    continue;

                serverAttributes.Add(attribute.Key, value);
            }

            return serverAttributes;
        }

        protected virtual ILinkDataAttibuteConverter? FindAttributeConverter(Type type)
        {
            if (_resolvedAttributeConverters.TryGetValue(type, out var converter))
                return converter;

            foreach (var attributeConverter in _attibuteConverters)
            {
                if (!attributeConverter.CanConvert(type))
                    continue;

                _resolvedAttributeConverters.TryAdd(type, attributeConverter);
                return attributeConverter;
            }

            return null;
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
