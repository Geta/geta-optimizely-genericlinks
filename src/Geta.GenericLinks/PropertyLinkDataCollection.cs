using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Core.Transfer;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Geta.GenericLinks.Html;
using Geta.GenericLinks.Extensions;

namespace Geta.GenericLinks
{
    public abstract class PropertyLinkDataCollection : PropertyLongString
    {
        protected PropertyLinkDataCollection()
        {
        }

        protected PropertyLinkDataCollection(string value) : base(value)
        {
        }
    }

    public abstract class PropertyLinkDataCollection<TLinkData> : PropertyLinkDataCollection, IReferenceMap, IEnumerable<TLinkData>, IEnumerable
        where TLinkData : ILinkData, new()
    {
        private static readonly IServiceProvider _serviceProvider;

        private readonly IUrlResolver _urlResolver;
        private readonly Injected<IUrlResolver> _injectedUrlResolver;
        private readonly IPrincipalAccessor _principalAccessor;
        private readonly IPermanentLinkMapper _permanentLinkMapper;
        private readonly IAttributeSanitizer _attributeSanitizer;
        private readonly ILinkHtmlSerializer _htmlSerializer;

        private LinkDataCollection<TLinkData>? _linkItemCollection;

        static PropertyLinkDataCollection()
        {
            _serviceProvider = ServiceLocator.Current;
        }

        public PropertyLinkDataCollection()
            : this(
                   _serviceProvider.GetInstance<IUrlResolver>(),
                   _serviceProvider.GetInstance<IPrincipalAccessor>(),
                   _serviceProvider.GetInstance<IPermanentLinkMapper>(),
                   _serviceProvider.GetInstance<IAttributeSanitizer>(),
                   _serviceProvider.GetInstance<ILinkHtmlSerializer>())
        {
        }

        public PropertyLinkDataCollection(LinkDataCollection<TLinkData> linkItemCollection)
            : this(
                  linkItemCollection,
                   _serviceProvider.GetInstance<IUrlResolver>(),
                   _serviceProvider.GetInstance<IPrincipalAccessor>(),
                   _serviceProvider.GetInstance<IPermanentLinkMapper>(),
                   _serviceProvider.GetInstance<IAttributeSanitizer>(),
                   _serviceProvider.GetInstance<ILinkHtmlSerializer>())
        {
        }

        public PropertyLinkDataCollection(
            IUrlResolver urlResolver,
            IPrincipalAccessor principalAccessor,
            IPermanentLinkMapper permanentLinkMapper,
            IAttributeSanitizer attributeSanitizer,
            ILinkHtmlSerializer htmlSerializer)
            : base(string.Empty)
        {
            _urlResolver = urlResolver;
            _principalAccessor = principalAccessor;
            _permanentLinkMapper = permanentLinkMapper;
            _attributeSanitizer = attributeSanitizer;
            _htmlSerializer = htmlSerializer;
            _injectedUrlResolver = new Injected<IUrlResolver>(urlResolver);
        }

        public PropertyLinkDataCollection(
            LinkDataCollection<TLinkData> linkItemCollection,
            IUrlResolver urlResolver,
            IPrincipalAccessor principalAccessor,
            IPermanentLinkMapper permanentLinkMapper,
            IAttributeSanitizer attributeSanitizer,
            ILinkHtmlSerializer htmlSerializer)
        {
            _linkItemCollection = linkItemCollection;
            _urlResolver = urlResolver;
            _principalAccessor = principalAccessor;
            _permanentLinkMapper = permanentLinkMapper;
            _attributeSanitizer = attributeSanitizer;
            _htmlSerializer = htmlSerializer;
            _injectedUrlResolver = new Injected<IUrlResolver>(urlResolver);
        }

        [XmlIgnore]
        public virtual LinkDataCollection<TLinkData>? Links
        {
            get => _linkItemCollection;
            set
            {
                ThrowIfReadOnly();

                if (value is null)
                {
                    Clear();
                }
                else if (_linkItemCollection != value)
                {
                    _linkItemCollection = value;
                    ModifiedNoCheck();
                }
            }
        }

        public override bool IsModified
        {
            get
            {
                if (base.IsModified)
                    return true;

                if (_linkItemCollection is not null)
                    return _linkItemCollection.IsModified;

                return false;
            }
            set => base.IsModified = value;
        }

        public override bool IsNull
        {
            get
            {
                if (_linkItemCollection is not null && _linkItemCollection.Count > 0)
                    return false;

                return !((ILazyProperty)this).HasLazyValue;
            }
        }

        public override PropertyDataType Type => PropertyDataType.LinkCollection;
        public override Type PropertyValueType => typeof(LinkDataCollection<TLinkData>);

        public override object? Value
        {
            get => _linkItemCollection;
            set
            {
                SetPropertyValue(value, delegate
                {
                    var collection = value as LinkDataCollection<TLinkData>;
                    if (collection is not null || value is null)
                    {
                        _linkItemCollection = collection;
                    }
                    else
                    {
                        if (value is not string text)
                            throw new ArgumentNullException("Passed object must be of type LinkItemCollection<TLinkData> or string.");

                        ParseToSelf(text);
                    }
                });
            }
        }
        protected override string LongString
        {
            get => _htmlSerializer.Serialize(_linkItemCollection, StringMode.InternalMode);
            set => base.LongString = value;
        }

        public override void ParseToSelf(string value)
        {
            ThrowIfReadOnly();

            if (QualifyAsNullString(value))
            {
                _linkItemCollection = null;
                return;
            }

            _linkItemCollection = ParseToLinkCollection(value);
            ModifiedNoCheck();
        }

        public override object? SaveData(PropertyDataCollection properties)
        {
            return ToLongString();
        }

        public override void MakeReadOnly()
        {
            base.MakeReadOnly();

            if (_linkItemCollection is null)
                return;

            if (_linkItemCollection.IsReadOnly)
                return;

            _linkItemCollection.MakeReadOnly();
        }

        public override string ToWebString()
        {
            return _htmlSerializer.Serialize(_linkItemCollection, StringMode.ViewMode);
        }

        public virtual IList<Guid>? ReferencedPermanentLinkIds
        {
            get
            {
                if (IsNull)
                    return Array.Empty<Guid>();

                return _linkItemCollection?.ReferencedPermanentLinkIds;
            }
        }

        public override PropertyData CreateWritableClone()
        {
            var clone = (PropertyLinkDataCollection<TLinkData>)base.CreateWritableClone();

            if (_linkItemCollection is not null)
                clone.SetLinkItems(_linkItemCollection.CreateWritableClone());

            return clone;
        }

        public override PropertyData Copy()
        {
            var collection = (PropertyLinkDataCollection<TLinkData>)base.Copy();

            LinkDataCollection<TLinkData> links;

            if (_linkItemCollection is null)
            {
                links = new LinkDataCollection<TLinkData>();
            }
            else
            {
                links = new LinkDataCollection<TLinkData>(_linkItemCollection);
            }

            collection.SetLinkItems(links);

            if (IsReadOnly)
            {
                links.MakeReadOnly();
            }

            return collection;
        }

        public override void LoadData(object value)
        {
            if (value is LinkDataCollection<TLinkData>)
            {
                Value = value;
            }
            else
            {
                _linkItemCollection = ParseToLinkCollection((string)value);
            }
        }

        public virtual void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
        {
            if (_linkItemCollection is null)
                return;

            _linkItemCollection.RemapPermanentLinkReferences(idMap);
        }

        public virtual IEnumerator<TLinkData> GetEnumerator()
        {
            if (_linkItemCollection is null)
                return Enumerator.Empty<TLinkData>();

            return _linkItemCollection.GetEnumerator();
        }

        protected override void SetDefaultValue()
        {
            base.SetDefaultValue();
            _linkItemCollection = null;
        }

        protected virtual LinkDataCollection<TLinkData>? ParseToLinkCollection(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var linkItemCollection = new LinkDataCollection<TLinkData>();

            try
            {
                var linkElements = GetLinkElements(value);

                foreach (var element in linkElements)
                {
                    var linkItem = new TLinkData
                    {
                        Text = element.Value
                    };

                    var attributes = GetDefinedAttributes(element);

                    foreach (var attribute in attributes)
                    {
                        linkItem.Attributes.Add(attribute.Key, attribute.Value);
                    }

                    linkItem.SetAttributes(attributes);
                    linkItemCollection.Add(linkItem);
                }

                return linkItemCollection;
            }
            catch (XmlException xmlException)
            {
                throw new InvalidPropertyValueException(nameof(PropertyLinkDataCollection<TLinkData>), value, xmlException);
            }
            catch (ArgumentException argumentException)
            {
                throw new InvalidPropertyValueException(nameof(PropertyLinkDataCollection<TLinkData>), value, argumentException);
            }
        }

        protected virtual string? ToLongString()
        {
            if (_linkItemCollection is null)
                return null;

            if (_linkItemCollection.Count == 0)
                return null;

            var elements = new List<XElement>(_linkItemCollection.Count);

            foreach (var linkItem in _linkItemCollection)
            {
                var attributes = linkItem.GetAttributes();
                var element = new XElement("a", GetElementAttributes(attributes));

                if (!string.IsNullOrWhiteSpace(linkItem.Href))
                    element.SetAttributeValue("href", _urlResolver.GetPermanent(linkItem.Href, enableFallback: true));

                if (!string.IsNullOrWhiteSpace(linkItem.Text))
                    element.SetValue(_attributeSanitizer.Sanitize(linkItem.Text));

                elements.Add(element);
            }

            return new XElement("links", elements).ToString(SaveOptions.DisableFormatting);
        }

        protected virtual IEnumerable<XAttribute> GetElementAttributes(IDictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                yield return new XAttribute(attribute.Key, _attributeSanitizer.Sanitize(attribute.Value));
            }
        }

        protected virtual IEnumerable<XElement> GetLinkElements(string value)
        {
            var rootElement = XElement.Parse(value);

            foreach (var element in rootElement.Descendants())
            {
                if (element.Name != "a")
                    continue;

                yield return element;
            }
        }

        protected virtual IDictionary<string, string> GetDefinedAttributes(XElement element)
        {
            var attributes = element.Attributes();
            var attributeLookup = new Dictionary<string, string>();

            foreach (var attribute in attributes)
            {
                if (string.IsNullOrEmpty(attribute.Value))
                    continue;

                var name = attribute.Name.LocalName;
                if (attributeLookup.ContainsKey(name))
                    continue;

                attributeLookup.Add(name, attribute.Value);
            }

            return attributeLookup;
        }

        internal void SetLinkItems(LinkDataCollection<TLinkData> linkItems)
        {
            _linkItemCollection = linkItems;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_linkItemCollection is null)
                return Enumerator.Empty<TLinkData>();

            return _linkItemCollection.GetEnumerator();
        }
    }
}
