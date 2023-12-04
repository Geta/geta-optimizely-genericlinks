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
