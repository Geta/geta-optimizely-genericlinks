// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Core.Transfer;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Geta.Optimizely.GenericLinks.Html;

namespace Geta.Optimizely.GenericLinks
{
    public abstract class PropertyLinkData : PropertyLinkBase
    {
        protected PropertyLinkData()
        {
        }

        protected PropertyLinkData(string value) : base(value)
        {
        }
    }

    public abstract class PropertyLinkData<TLinkData> : PropertyLinkData, IReferenceMap
        where TLinkData : LinkData, new()
    {
        private readonly IUrlResolver _urlResolver;
        private readonly IAttributeSanitizer _attributeSanitizer;
        private readonly ILinkHtmlSerializer _htmlSerializer;

        private TLinkData? _linkItem;

        protected PropertyLinkData()
            : this(
                   ServiceLocator.Current.GetInstance<IUrlResolver>(),
                   ServiceLocator.Current.GetInstance<IAttributeSanitizer>(),
                   ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>())
        {
        }

        protected PropertyLinkData(TLinkData linkItem)
            : this(
                  linkItem,
                   ServiceLocator.Current.GetInstance<IUrlResolver>(),
                   ServiceLocator.Current.GetInstance<IAttributeSanitizer>(),
                   ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>())
        {
        }

        protected PropertyLinkData(
            IUrlResolver urlResolver,
            IAttributeSanitizer attributeSanitizer,
            ILinkHtmlSerializer htmlSerializer)
            : base(string.Empty)
        {
            _urlResolver = urlResolver;
            _attributeSanitizer = attributeSanitizer;
            _htmlSerializer = htmlSerializer;
        }

        protected PropertyLinkData(
            TLinkData linkItem,
            IUrlResolver urlResolver,
            IAttributeSanitizer attributeSanitizer,
            ILinkHtmlSerializer htmlSerializer)
        {
            _linkItem = linkItem;
            _urlResolver = urlResolver;
            _attributeSanitizer = attributeSanitizer;
            _htmlSerializer = htmlSerializer;
        }

        [XmlIgnore]
        public virtual TLinkData? Link
        {
            get => _linkItem;
            set
            {
                ThrowIfReadOnly();

                if (value is null)
                {
                    Clear();
                    return;
                }

                _linkItem = value;
                ModifiedNoCheck();
            }
        }

        public override bool IsModified
        {
            get
            {
                if (base.IsModified)
                    return true;

                if (_linkItem is not null)
                    return _linkItem.IsModified;

                return false;
            }
            set => base.IsModified = value;
        }


        public override bool IsNull
        {
            get
            {
                if (_linkItem is not null && !string.IsNullOrEmpty(_linkItem.Href))
                    return false;

                return !((ILazyProperty)this).HasLazyValue;
            }
        }

        public override PropertyDataType Type => PropertyDataType.LinkCollection;
        public override Type PropertyValueType => typeof(TLinkData);

        public override object? Value
        {
            get => _linkItem;
            set
            {
                SetPropertyValue(value, delegate
                {
                    var linkData = value as TLinkData;
                    if (linkData is not null || value is null)
                    {
                        _linkItem = linkData;
                    }
                    else
                    {
                        if (value is not string text)
                            throw new ArgumentException("Passed object must be of type TLinkData or string.", nameof(value));

                        ParseToSelf(text);
                    }
                });
            }
        }
        protected override string LongString
        {
            get => _htmlSerializer.Serialize(_linkItem, StringMode.InternalMode);
            set => base.LongString = value;
        }

        public override void ParseToSelf(string value)
        {
            ThrowIfReadOnly();

            if (QualifyAsNullString(value))
            {
                _linkItem = null;
                return;
            }

            _linkItem = ParseToLink(value);
            ModifiedNoCheck();
        }

        public override string ToWebString()
        {
            return _htmlSerializer.Serialize(_linkItem, StringMode.ViewMode);
        }

        public virtual IList<Guid>? ReferencedPermanentLinkIds
        {
            get
            {
                if (IsNull)
                    return Array.Empty<Guid>();

                return _linkItem?.ReferencedPermanentLinkIds;
            }
        }

        public override PropertyData CreateWritableClone()
        {
            var clone = (PropertyLinkData<TLinkData>)base.CreateWritableClone();

            if (_linkItem is not null)
                clone.SetLinkItem((TLinkData)_linkItem.Clone());

            return clone;
        }

        public override PropertyData Copy()
        {
            var property = (PropertyLinkData<TLinkData>)base.Copy();

            TLinkData? link;

            if (_linkItem is null)
            {
                link = null;
            }
            else
            {
                link = (TLinkData)_linkItem.Clone();
            }

            property.SetLinkItem(link);

            if (IsReadOnly)
            {
                property.MakeReadOnly();
            }

            return property;
        }

        public override void LoadData(object value)
        {
            if (value is null)
            {
                _linkItem = null;
            }
            else if (value is TLinkData)
            {
                Value = value;
            }
            else
            {
                _linkItem = ParseToLink((string)value);
            }
        }

        public virtual void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
        {
            if (_linkItem is null)
                return;

            _linkItem.RemapPermanentLinkReferences(idMap);
        }

        protected override void SetDefaultValue()
        {
            base.SetDefaultValue();
            _linkItem = null;
        }

        protected virtual TLinkData? ParseToLink(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                var element = GetLinkElement(value);
                var linkItem = new TLinkData
                {
                    Text = element.Value
                };

                ParseAttributes(element, linkItem);
                return linkItem;
            }
            catch (XmlException xmlException)
            {
                throw new InvalidPropertyValueException(nameof(PropertyLinkData<TLinkData>), value, xmlException);
            }
            catch (ArgumentException argumentException)
            {
                throw new InvalidPropertyValueException(nameof(PropertyLinkData<TLinkData>), value, argumentException);
            }
        }

        protected override string? ToLongString()
        {
            if (_linkItem is null)
                return null;

            var element = CreateLinkElement(_linkItem);

            return new XElement("links", element).ToString(SaveOptions.DisableFormatting);
        }

        protected override string GetPermanentUrl(string href)
        {
            return _urlResolver.GetPermanent(href, enableFallback: true);
        }

        protected override string SanitizeValue(string value)
        {
            return _attributeSanitizer.Sanitize(value);
        }

        internal void SetLinkItem(TLinkData? linkItem)
        {
            _linkItem = linkItem;
        }
    }
}
