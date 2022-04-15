// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Web;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class FakeVirtualPathResolver : IVirtualPathResolver
    {
        public string ToAbsolute(string virtualPath)
        {
            return virtualPath;
        }

        public string ToAppRelative(string virtualPath)
        {
            return virtualPath;
        }
    }
}
