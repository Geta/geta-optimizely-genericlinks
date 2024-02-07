// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAbstraction;

namespace Geta.Optimizely.GenericLinks.Cms.Registration;

public class PropertyLinkDataCollectionDefinitionsLoader : DataDefinitionsLoaderBase
{
    public PropertyLinkDataCollectionDefinitionsLoader(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository)
        : base(typeof(PropertyLinkDataCollection), propertyDefinitionTypeRepository)
    {

    }
}
