// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using Geta.Optimizely.GenericLinks.Initialization;
using Geta.Optimizely.GenericLinks.Tests.Extensions;
using Geta.Optimizely.GenericLinks.Tests.Models;
using Geta.Optimizely.GenericLinks.Tests.Services;
using Xunit;

namespace Geta.Optimizely.GenericLinks.Tests;

public class ContentScannerExtensionTests
{

    [Fact]
    public void ContentScannerExtension_can_AssignValuesToProperties()
    {
        var collectionPropertyType = typeof(PropertyTestCollection);
        var singlePropertyType = typeof(PropertyTestLinkData);
        var definitionTypes = new List<PropertyDefinitionType>
        {
            // Ids lower than 1000 are considered system types, belonging to Optimizely
            collectionPropertyType.ToDefinition(1001, PropertyDataType.LinkCollection),
            singlePropertyType.ToDefinition(1002, PropertyDataType.LinkCollection),
        };

        var definitionRepository = new InMemoryPropertyDefinitionTypeRepository(definitionTypes);
        var subject = new GenericLinkContentScannerExtension(definitionRepository);
        var contentTypeModel = new ContentTypeModel
        {
            Name = "Test page",
            ModelType = typeof(TestPage)
        };
        var collectionPropertyDefinition = new PropertyDefinitionModel
        {
            Name = "Test links",
            Type = typeof(LinkDataCollection<TestLinkData>),
        };
        var singlePropertyDefinition = new PropertyDefinitionModel
        {
            Name = "Test link",
            Type = typeof(TestLinkData),
        };

        contentTypeModel.PropertyDefinitionModels.Add(collectionPropertyDefinition);
        contentTypeModel.PropertyDefinitionModels.Add(singlePropertyDefinition);

        subject.AssignValuesToProperties(contentTypeModel);

        Assert.Equal(typeof(PropertyTestCollection), collectionPropertyDefinition.BackingType);
        Assert.Equal(typeof(PropertyTestLinkData), singlePropertyDefinition.BackingType);
    }
}
