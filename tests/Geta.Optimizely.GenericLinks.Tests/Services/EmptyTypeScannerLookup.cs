using EPiServer.Framework.TypeScanner;
using System;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks.Tests.Services;

internal class EmptyTypeScannerLookup : ITypeScannerLookup
{
    public IEnumerable<Type> AllTypes => Array.Empty<Type>();
}
