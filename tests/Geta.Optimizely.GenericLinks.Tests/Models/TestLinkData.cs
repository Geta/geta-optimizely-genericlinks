// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Geta.Optimizely.GenericLinks;

namespace Geta.Optimizely.GenericLinks.Tests.Models
{
    public class TestLinkData : LinkData
    {
        public override object Clone()
        {
            var item = new TestLinkData
            {
                Text = Text,
            };

            item.SetAttributes(Attributes);

            return item;
        }
    }
}
