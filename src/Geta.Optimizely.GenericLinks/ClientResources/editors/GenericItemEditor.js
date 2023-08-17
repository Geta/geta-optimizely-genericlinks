// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

define("genericLinks/editors/GenericItemEditor", [
  "dojo/_base/declare",
  "dojo/_base/lang",
  "dojo/aspect",
  "dojo/when",
  "genericLinks/widget/_SelectorBase",
  "epi-cms/contentediting/command/ItemEdit",
  "genericLinks/viewmodel/ItemCollectionViewModel",
  "genericLinks/viewmodel/LinkItemModel",
], function (
  declare,
  lang,
  aspect,
  when,
  _SelectorBase,
  ItemEditCommand,
  ItemCollectionViewModel,
  LinkItemModel
) {
  return declare([_SelectorBase], {
    _deferredValue: null,
    postCreate: function () {
      this.inherited(arguments);
      this.set(
        "model",
        new ItemCollectionViewModel(this.value, {
          itemModelType: this.itemModelType,
          readOnly: this.readOnly,
          canEdit: true,
        })
      );
      if (this._deferredValue) {
        this._setValueAttr(this._deferredValue);
        this._deferredValue = null;
      }
      if (!this.value) {
        this._updateDisplayNode(null);
      }
      this.setupCommands();
      this.setupEvents();
    },
    setupCommands: function () {
      this._editItemCommand = new ItemEditCommand({
        isAvailable: true,
        model: this.model,
        dialogContentParams: this.commandOptions
          ? this.commandOptions.dialogContentParams
          : {},
      });
      this.own(this._editItemCommand);
    },
    setupEvents: function () {
      this.own(
        aspect.after(
          this._editItemCommand,
          "onDialogOpen",
          function () {
            this.set("isShowingChildDialog", true);
            this.focus();
          }.bind(this)
        ),
        aspect.after(
          this._editItemCommand,
          "onDialogHideComplete",
          function () {
            this.set("isShowingChildDialog", false);

            // Fix for quirk in CMS 12.19 where closing the dialog triggers autosave.
            // This stops the value from being null at the incorrect moment.
            this.value = this.getValue();
          }.bind(this)
        ),
        this.model.on(
          "changed",
          lang.hitch(this, function () {
            const value = this.getValue();
            this.updateDisplay(value);
            this.onChange((this.value = value));
          })
        )
      );
    },
    updateDisplay: function (value) {
      if (value instanceof Array) {
        if (value.length > 0) {
          return this._updateDisplayNode({ name: value[0].text });
        }
      } else if (value) {
        return this._updateDisplayNode({ name: value.text });
      }
      return this._updateDisplayNode(null);
    },
    _onDropData: function (dndData, source, nodes, copy) {
      var dropItem = dndData ? (dndData.length ? dndData[0] : dndData) : null;
      if (dropItem) {
        this.onDropping();
        when(
          dropItem.data,
          lang.hitch(this, function (resolvedValue) {
            this.model.clear();
            this.model.addTo(resolvedValue, null, false);
            this.updateDisplay(resolvedValue);
          })
        );
        this.focus();
      }
    },
    _setValueAttr: function (value) {
      if (!this.model) {
        this._deferredValue = value;
        return;
      }
      if (!value || (value instanceof Array && value.length === 0)) {
        this.model.set("data", []);
        this.updateDisplay(null);
        return;
      }
      if (value && !(value instanceof Array)) {
        value = [value];
      }
      this.value = value;
      this.model.set("data", value);
      this.updateDisplay(value);
    },

    isValid: function () {
      return (
        !this.required ||
        (this.model.get("data") && this.model.get("data").length > 0)
      );
    },

    _onButtonClick: function () {
      this.model.selectFirstItem();
      this._editItemCommand.set("canExecute", true);
      this._editItemCommand.set("value", this.model.get("selectedItem"));
      this._editItemCommand.execute();
    },

    getValue: function () {
      return this.model.get("value") || this.getEmptyValue();
    },

    getEmptyValue: function () {
      return [];
    },

    clearValue: function () {
      this.inherited(arguments);
      this.model.clear();
    },

    _setSelectedContentNameAttr: function (value) {
      const name = value || "{unnamed}";

      this.selectedContentNameNode.innerHTML =
        "<span class='dijitInline dijitIcon epi-iconLink epi-objectIcon'></span>&nbsp;" +
        name;
      this._updateDisplayNodeTitle();
    },
  });
});
