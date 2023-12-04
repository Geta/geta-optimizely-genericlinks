// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer.DataAbstraction;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    internal sealed class NullBackingTypeResolver : IBackingTypeResolver
    {
        public Type? Resolve(Type type)
        {
            return null;
        }
    }
}
