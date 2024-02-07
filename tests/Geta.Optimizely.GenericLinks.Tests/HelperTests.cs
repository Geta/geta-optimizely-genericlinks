// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Geta.Optimizely.GenericLinks.Helpers;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests;

public class HelperTests
{
    [Fact]
    public void UriHelper_can_CreateUri()
    {
        var result = UriHelper.CreateUri("http://localhost");
        Assert.NotNull(result);

        result = UriHelper.CreateUri(null);
        Assert.Null(result);
    }

    [Fact]
    public void UriHelper_throws_UriFormatException()
    {
        Assert.Throws<UriFormatException>(() => UriHelper.CreateUri("http://"));
        Assert.Throws<UriFormatException>(() => UriHelper.CreateUri("http://localhost:80:80"));
        Assert.Throws<UriFormatException>(() => UriHelper.CreateUri("http://../temp/mydocument.txt"));
    }
}
