// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAbstraction;
using System;

namespace Geta.Optimizely.GenericLinks.Tests.Services;

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
