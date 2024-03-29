// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Geta.Optimizely.GenericLinks.Cms.EditorDescriptors;

namespace Geta.Optimizely.GenericLinks.Cms.Metadata;

public class LinkDataMetadataExtender : IMetadataExtender
{
    private readonly bool _singleItem;
    private readonly string _customIdentifier;
    private readonly string _modelTypeIdentifier;
    private readonly string[] _allowedDragAndDropTypes;

    public LinkDataMetadataExtender(Type extenderType, bool singleItem, IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors)
    {
        _singleItem = singleItem;
        _customIdentifier = extenderType.FullName!.ToLower() ?? string.Empty;
        if (!singleItem)
            _customIdentifier += "collection";

        _modelTypeIdentifier = typeof(LinkModel<>).MakeGenericType(extenderType).FullName!.ToLower();
        _allowedDragAndDropTypes = contentRepositoryDescriptors.Where(d => d.LinkableTypes != null)
                                                               .SelectMany(d => d.LinkableTypes)
                                                               .Where(t => t.FullName is not null)
                                                               .Select(t => t.FullName!.ToLower() + ".link")
                                                               .Distinct()
                                                               .ToArray();
    }

    public virtual void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        if (metadata is LinkDataMetadata)
            return;

        var collectionOptions = new CollectionEditorOptions
        {
            ItemModelType = "genericLinks/viewmodel/LinkItemModel",
            CustomTypeIdentifier = _customIdentifier,
        };
        var commandOptions = new CommandOptions
        {
            DialogContentParams = new DialogContentOptions
            {
                ModelType = _modelTypeIdentifier,
                BaseClass = "epi-link-item",
            }
        };

        var editingClass = _singleItem ? "genericLinks/editors/GenericItemEditor"
                                       : "genericLinks/editors/GenericCollectionEditor";

        metadata.ClientEditingClass = editingClass;
        metadata.OverlayConfiguration["modelParams"] = collectionOptions;
        metadata.OverlayConfiguration[EditorDescriptorConstants.AllowedDndTypesKey] = _allowedDragAndDropTypes;
        metadata.EditorConfiguration["itemModelType"] = collectionOptions.ItemModelType;
        metadata.EditorConfiguration["customTypeIdentifier"] = collectionOptions.CustomTypeIdentifier;
        metadata.EditorConfiguration["commandOptions"] = commandOptions;
        metadata.EditorConfiguration[EditorDescriptorConstants.AllowedDndTypesKey] = _allowedDragAndDropTypes;
    }
}
