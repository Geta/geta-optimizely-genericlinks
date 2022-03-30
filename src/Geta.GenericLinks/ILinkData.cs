using EPiServer.Core.Transfer;
using System;
using System.Collections.Generic;

namespace Geta.GenericLinks
{
    public interface ILinkData : ICloneable, IReferenceMap
    {
        IDictionary<string, string> Attributes { get; }

        string? Text { get; set; }

        string? Href { get; set; }

        string? Target { get; set; }

        string? Title { get; set; }

        void SetAttributes(IDictionary<string, string> attributes);
        IDictionary<string, string> GetAttributes();
    }
}
