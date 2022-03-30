using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using Geta.GenericLinks.Cms.EditorDescriptors;
using Geta.GenericLinks.Cms.EditorModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geta.GenericLinks.Cms.Metadata
{
    public class LinkDataMetadataExtender : IMetadataExtender
    {
        private readonly Type _extenderType;
        private readonly string _customIdentifier;
        private readonly string _modelTypeIdentifier;
        private string[] _allowedDragAndDropTypes;

        public LinkDataMetadataExtender(Type extenderType, IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors)
        {
            _extenderType = extenderType;
            _customIdentifier = _extenderType.FullName!.ToLower();
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

            metadata.ClientEditingClass = "genericLinks/editors/GenericCollectionEditor";
            metadata.OverlayConfiguration["modelParams"] = collectionOptions;
            metadata.OverlayConfiguration[EditorDescriptorConstants.AllowedDndTypesKey] = _allowedDragAndDropTypes;
            metadata.EditorConfiguration["itemModelType"] = collectionOptions.ItemModelType;
            metadata.EditorConfiguration["customTypeIdentifier"] = collectionOptions.CustomTypeIdentifier;
            metadata.EditorConfiguration["commandOptions"] = commandOptions;
            metadata.EditorConfiguration[EditorDescriptorConstants.AllowedDndTypesKey] = _allowedDragAndDropTypes;
        }
    }
}
