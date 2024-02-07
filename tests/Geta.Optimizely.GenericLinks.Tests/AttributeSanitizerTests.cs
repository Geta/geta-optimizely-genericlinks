// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Geta.Optimizely.GenericLinks.Html;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests;

public class AttributeSanitizerTests
{
    [Fact]
    public void AttributeSanitizer_can_Sanitize()
    {
        var subject = new DefaultAttributeSanitizer();
        var allowedCharacters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ`abcdefghijklmnopqrstuvwxyzåäö{|}~";
        var illegalCharacters = "\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\u000A\u000B\u000C\u000D\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001E\u001F";

        var allowedSanitized = subject.Sanitize(allowedCharacters);
        Assert.Equal(allowedCharacters, allowedSanitized);

        var illegalSanitized = subject.Sanitize(illegalCharacters);
        Assert.Equal(0, illegalSanitized.Length);
    }
}
