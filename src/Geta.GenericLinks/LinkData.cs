using EPiServer.DataAnnotations;
using EPiServer.Web;
using Geta.GenericLinks.Extensions;
using Geta.GenericLinks.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Geta.GenericLinks
{
    public abstract class LinkData : ILinkData
    {
        private readonly IDictionary<string, string> _attributes;
        private readonly IDictionary<string, string> _attributeKeys;

        private bool _isModified;

        public LinkData()
        {
            _attributes = new Dictionary<string, string>(4);
            _attributeKeys = new Dictionary<string, string>(4);
        }

        [Required]
        [Display(Order = 100)]
        public virtual string? Text { get; set; }

        [Display(Order = 200)]
        public virtual string? Title
        {
            get => GetAttribute();
            set => SetAttribute(value);
        }

        [Display(Order = 300)]
        public virtual string? Target
        {
            get => GetAttribute();
            set => SetAttribute(value);
        }

        [Required]
        [Display(Order = 400)]
        public virtual string? Href
        {
            get => GetAttribute();
            set
            {
                UriHelper.CreateUri(value);
                SetAttribute(value);
            }
        }

        [ScaffoldColumn(false)]
        public virtual IDictionary<string, string> Attributes => _attributes;

        [Ignore]
        public virtual bool IsModified
        {
            get => _isModified;
        }

        [Ignore]
        [ScaffoldColumn(false)]
        public virtual IList<Guid> ReferencedPermanentLinkIds
        {
            get
            {
                var href = Href;

                if (string.IsNullOrEmpty(href))
                    return new List<Guid>(0);

                var guid = PermanentLinkUtility.GetGuid(href);
                if (guid == Guid.Empty)
                    return new List<Guid>(0);

                return new List<Guid>(1)
                {
                    guid
                };
            }
        }

        public virtual IDictionary<string, string> GetAttributes()
        {
            return Attributes;
        }

        public virtual void SetAttributes(IDictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                Attributes.TryGetValue(attribute.Key, out var existingValue);
                if (existingValue == attribute.Value)
                    continue;

                Attributes[attribute.Key] = attribute.Value;
            }
        }

        public virtual void SetModified(bool isModified)
        {
            _isModified = isModified;
        }

        public virtual void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
        {
            var href = Href;
            if (string.IsNullOrEmpty(href))
                return;

            var guid = PermanentLinkUtility.GetGuid(href);
            if (guid == Guid.Empty)
                return;

            if (!idMap.TryGetValue(guid, out var value))
                return;

            if (value == Guid.Empty)
                value = idMap[guid] = Guid.NewGuid();

            Href = PermanentLinkUtility.ChangeGuid(href, value);
        }       

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual string? GetAttribute([CallerMemberName] string? key = null)
        {
            if (key is null)
                return null;

            if (Attributes.TryGetValue(GetAttributeKey(key), out var value))
                return value;

            return null;
        }

        public virtual void SetAttribute(string? value, [CallerMemberName] string? key = null)
        {
            if (key is null)
                return;

            var attributeKey = GetAttributeKey(key);

            if (value is null)
            {
                if (!Attributes.ContainsKey(attributeKey))
                    return;

                Attributes.Remove(attributeKey);
                _isModified = true;
            }
            else
            {
                Attributes.TryGetValue(attributeKey, out var existingValue);
                if (existingValue == value)
                    return;

                Attributes[attributeKey] = value;
                _isModified = true;
            }
        }

        protected virtual string GetAttributeKey(string key)
        {
            if (_attributeKeys.TryGetValue(key, out var attributeKey))
                return attributeKey;

            attributeKey = key.ToCamel();
            _attributeKeys.TryAdd(key, attributeKey);

            return attributeKey;
        }
    }
}
