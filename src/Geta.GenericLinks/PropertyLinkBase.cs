using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Geta.GenericLinks
{
    public abstract class PropertyLinkBase : PropertyLongString
    {
        protected PropertyLinkBase()
        {
        }

        protected PropertyLinkBase(string value) : base(value)
        {
        }

        protected virtual IEnumerable<XAttribute> GetElementAttributes(IDictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                var value = SanitizeValue(attribute.Value);
                if (string.IsNullOrEmpty(value))
                    continue;

                yield return new XAttribute(attribute.Key, value);
            }
        }

        protected abstract string SanitizeValue(string value);

        protected abstract string GetPermanentUrl(string href);

        protected virtual XElement GetElement(ILinkData linkItem)
        {
            var attributes = linkItem.GetAttributes();
            var element = new XElement("a", GetElementAttributes(attributes));

            if (!string.IsNullOrWhiteSpace(linkItem.Href))
                element.SetAttributeValue("href", GetPermanentUrl(linkItem.Href));

            if (!string.IsNullOrWhiteSpace(linkItem.Text))
                element.SetValue(SanitizeValue(linkItem.Text));

            return element;
        }

        protected virtual XElement GetLinkElement(string value)
        {
            var rootElement = XElement.Parse(value);

            foreach (var element in rootElement.Descendants())
            {
                if (element.Name != "a")
                    continue;

                return element;
            }

            throw new InvalidOperationException("Malformed data");
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
