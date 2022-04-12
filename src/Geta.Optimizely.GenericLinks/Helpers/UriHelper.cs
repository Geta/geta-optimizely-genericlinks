// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Web;
using System;
using System.Globalization;

namespace Geta.Optimizely.GenericLinks.Helpers
{
    public static class UriHelper
    {
        public static Uri? CreateUri(string? link)
        {
            if (string.IsNullOrEmpty(link))
                return null;

            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var result))
            {
                throw new UriFormatException(string.Format(CultureInfo.InvariantCulture, "Malformed URI? Could not create Uri from link with value '{0}'.", link));
            }

            if (result.IsAbsoluteUri)
            {
                _ = result.Authority;
            }
            else if (UriUtil.IsSchemeSpecified(link))
            {
                throw new UriFormatException("Malformed URI? Absolute URI (scheme specified) which is not recognized as such by System.Uri.");
            }

            return result;
        }
    }
}
