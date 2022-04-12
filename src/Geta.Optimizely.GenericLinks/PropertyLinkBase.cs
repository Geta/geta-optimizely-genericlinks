using EPiServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Geta.Optimizely.GenericLinks
{
    public abstract class PropertyLinkBase : PropertyLongString
    {
        protected PropertyLinkBase()
        {
        }

        protected PropertyLinkBase(string value) : base(value)
        {
        }

        public override object? SaveData(PropertyDataCollection properties)
        {
            return ToLongString();
        }

        protected abstract string? ToLongString();

        protected abstract string SanitizeValue(string value);

        protected abstract string GetPermanentUrl(string href);

        protected virtual void ParseAttributes(XElement element, ILinkData linkItem)
        {
            var attributes = GetDefinedAttributes(element);

            foreach (var attribute in attributes)
            {
                linkItem.Attributes.Add(attribute.Key, attribute.Value);
            }

            linkItem.SetAttributes(attributes);
        }

        protected virtual XElement CreateLinkElement(ILinkData linkItem)
        {
            var attributes = linkItem.GetAttributes();
            var element = new XElement("a", CreateElementAttributes(attributes));

            if (!string.IsNullOrWhiteSpace(linkItem.Href))
                element.SetAttributeValue("href", GetPermanentUrl(linkItem.Href));

            if (!string.IsNullOrWhiteSpace(linkItem.Text))
                element.SetValue(SanitizeValue(linkItem.Text));

            return element;
        }

        protected virtual IEnumerable<XAttribute> CreateElementAttributes(IDictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                var value = SanitizeValue(attribute.Value);
                if (string.IsNullOrEmpty(value))
                    continue;

                yield return new XAttribute(attribute.Key, value);
            }
        }

        protected virtual XElement GetLinkElement(string value)
        {
            return GetLinkElements(value).First();
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

    }
}
