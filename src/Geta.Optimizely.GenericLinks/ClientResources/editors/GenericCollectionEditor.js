// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

define("genericLinks/editors/GenericCollectionEditor", [
  "dojo/_base/declare",
  "epi-cms/dgrid/formatters",
  "epi/shell/TypeDescriptorManager",
  "epi-cms/contentediting/editors/ItemCollectionEditor",
  "genericLinks/viewmodel/ItemCollectionViewModel",
], function (
  declare,
  formatters,
  TypeDescriptorManager,
  ItemCollectionEditor,
  ItemCollectionViewModel
) {
  return declare([ItemCollectionEditor], {
    postCreate: function () {
      this.set(
        "model",
        new ItemCollectionViewModel(this.value, {
          itemModelType: this.itemModelType,
          readOnly: this.readOnly,
        })
      );

      this.setupCommands();
      this.setupList();
      this.setupActionContainer();
      this.setupEvents();
    },
    setupList: function () {
      // summary:
      //      Initialization a list.
      // tags:
      //      protected

      var menu = {
          hasMenu: true,
          settings: { title: this.res ? this.res.menutooltip : "" },
        },
        linkAssembler = function (data, object, row) {
          return (
            "<div class='epi-rowIcon'><span class='dijitInline dijitIcon epi-iconLink epi-objectIcon'></span></div>" +
            data
          );
        },
        // Init grid
        settings = {
          selectionMode: "single",
          selectionEvents: "click,dgrid-cellfocusin",
          formatters: [
            formatters.contentItemFactory(
              (x) => x.text || x.title || "{unnamed}",
              (x) => x.title,
              (x) => x.typeIdentifier,
              menu
            ),
            linkAssembler,
          ],
          dndParams: {
            copyOnly: true,
            accept: this.get("allowedDndTypes"),
            creator: this._dndNodeCreator.bind(this),
            isSource: !this.readOnly,
          },
          dndSourceTypes: this.customTypeIdentifier
            ? [this.customTypeIdentifier]
            : [],
          consumer: this,
          commandCategory: null,
        },
        getDndType = function (object) {
          var types = TypeDescriptorManager.getAndConcatenateValues(
            this.dndSourceTypes,
            "dndTypes"
          );

          if (types.length === 0) {
            types = this.dndSourceTypes;
          }
          return types;
        };

      if (this.customTypeIdentifier) {
        settings._getDndType = getDndType;
      }

      this.own(
        (this.grid = new this._gridClass(settings, this.itemsContainer))
      );
      this.grid.contextMenu.addProvider(this.commandProvider);
      this.grid.set("showHeader", false);
      this.grid.renderArray(this.model.get("data"));
      this.grid.startup();
    },
  });
});
