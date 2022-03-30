using EPiServer.Shell;
using System;

namespace Geta.GenericLinks.Extensions
{
    internal static class UIDescriptorRegistryExtensions
    {
        public static string? GetTypeIdentifier(this UIDescriptorRegistry descriptorRegistry, Type type)
        {
            foreach (var identifier in descriptorRegistry.GetTypeIdentifiers(type))
                return identifier;

            return null;
        }
    }
}
