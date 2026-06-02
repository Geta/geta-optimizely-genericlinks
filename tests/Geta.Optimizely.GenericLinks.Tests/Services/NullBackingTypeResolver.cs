// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer.DataAbstraction;

namespace Geta.Optimizely.GenericLinks.Tests.Services;

internal sealed class NullBackingTypeResolver : IBackingTypeResolver
{
    public PropertyDefinitionTypeResolution Resolve(Type type)
    {
        throw new NotImplementedException("NullBackingTypeResolver should not be called directly — the interceptor should resolve link types before falling through.");
    }
}
