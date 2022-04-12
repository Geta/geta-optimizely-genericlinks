// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Shell;
using System;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Extensions
{
    internal static class UIDescriptorRegistryExtensions
    {
        public static string? GetTypeIdentifier(this UIDescriptorRegistry descriptorRegistry, Type type)
        {
            return descriptorRegistry.GetTypeIdentifiers(type)
                                     .FirstOrDefault();
        }
    }
}
