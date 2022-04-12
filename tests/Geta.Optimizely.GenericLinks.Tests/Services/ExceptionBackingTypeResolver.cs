using EPiServer.DataAbstraction;
using System;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class ExceptionBackingTypeResolver : IBackingTypeResolver
    {
        private readonly Exception _exception;

        public ExceptionBackingTypeResolver(Exception exception)
        {
            _exception = exception;
        }

        public Type Resolve(Type type)
        {
            throw _exception;
        }
    }
}
