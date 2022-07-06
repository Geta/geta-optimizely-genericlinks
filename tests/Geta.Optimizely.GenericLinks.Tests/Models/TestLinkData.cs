// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace Geta.Optimizely.GenericLinks.Tests.Models
{
    public class TestLinkData : LinkData
    {
        public virtual void CallSetAttribute(string key, string? value)
        {
            base.SetAttribute(value, key);
        }

        public virtual void CallGetAttributeKey(string key)
        {
            base.GetAttributeKey(key);
        }
    }
}
