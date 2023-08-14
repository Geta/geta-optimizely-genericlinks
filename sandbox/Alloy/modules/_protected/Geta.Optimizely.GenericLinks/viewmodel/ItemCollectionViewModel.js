// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

define("genericLinks/viewmodel/ItemCollectionViewModel", [
  // Dojo
  "dojo/_base/declare",
  "dojo/_base/lang",
  "dojo/Deferred",
  "dojo/promise/all",
  "dojo/when",

  // EPi CMS
  "epi-cms/contentediting/viewmodel/_ViewModelMixin",
  "genericLinks/viewmodel/LinkItemModel",
], function (
  declare,
  lang,
  Deferred,
  all,
  when,
  _ViewModelMixin,
  LinkItemModel
) {
  return declare([_ViewModelMixin], {
    _instanceId: null,
    _autoId: 0,
    _selectedItem: null,
    _data: null,
    itemModelClass: null,
    itemModelType: null,

    constructor: function (data, options) {
      declare.safeMixin(this, options);
    },

    postscript: function (data) {
      require([this.itemModelType], lang.hitch(this, function (modelClass) {
        this.itemModelClass = modelClass;
        this._instanceId = new Date().getTime();
        this._init(data);
      }));
    },

    moveNext: function () {
      this._move(1) && this.emit("changed");
    },

    movePrevious: function () {
      this._move(-1) && this.emit("changed");
    },

    remove: function () {
      if (!this.get("canEdit")) {
        return;
      }

      var index = this.getItemIndex(this._selectedItem);
      this._data.splice(index, 1);

      var len = this._data.length;
      this.set(
        "selectedItem",
        this._data[index >= len ? len - 1 : index] || null
      );

      this.emit("changed");
    },

    addTo: function (newItem, item, before) {
      if (!newItem) {
        return false;
      }
      if (this.getItemIndex(newItem) >= 0) {
        this.moveTo(newItem, null, false);
        this.emit("changed");
      } else {
        var self = this;
        when(self._createItemModel(newItem), function (model) {
          if (!item) {
            self._data.push(model);
          } else {
            var targetIndex = self.getItemIndex(item) + (before ? 0 : 1);
            self._data.splice(targetIndex, 0, model);
          }

          self.emit("changed", { uiChanged: false });
        });
      }
    },
    moveTo: function (selectedItem, item, before) {
      if (!selectedItem && !this._selectedItem) {
        return false;
      }

      this.set("selectedItem", selectedItem || this._selectedItem);

      var sourceIndex = this.getItemIndex(this._selectedItem);
      this._data.splice(sourceIndex, 1);

      var targetIndex =
        (item ? this.getItemIndex(item) : this._data.length - 1) +
        (before ? 0 : 1);
      this._data.splice(targetIndex, 0, this._selectedItem);

      this.emit("changed");
    },
    selectFirstItem: function () {
      if (this._data.length < 1) return;
      this._selectedItem = this._data[0];
    },
    clear: function () {
      this._data = [];

      this.set("selectedItem", null);

      this.emit("changed");
    },
    updateItemData: function (item) {
      var index = this.getItemIndex(this._selectedItem);
      if (index < 0) index = 0;
      var self = this;
      when(self._createItemModel(item), function (model) {
        self._selectedItem = self._data[index] = model;
        self.emit("changed");
      });
    },

    swap: function (sourceIndex, targetIndex) {
      if (!this._data[sourceIndex] || !this._data[targetIndex]) {
        return false;
      }

      var temp = this._data[sourceIndex];
      this._data[sourceIndex] = this._data[targetIndex];
      this._data[targetIndex] = temp;

      return true;
    },

    getItemIndex: function (item) {
      var index = -1;
      if (!item || !(item instanceof this.itemModelClass)) {
        return index;
      }

      this._data.some(function (i, o) {
        if (i.id === item.id) {
          index = o;
          return true;
        }

        return false;
      }, this);

      return index;
    },

    _init: function (data) {
      var self = this,
        itemDfds;

      this.set("selectedItem", null);

      if (!(data instanceof Array)) {
        this._data = [];
      } else {
        itemDfds = data.map(function (item) {
          return this._createItemModel(item);
        }, this);

        all(itemDfds).then(function (results) {
          self._data = results;
          self.emit("initCompleted");
        });
      }
    },

    _generateItemId: function () {
      return this._instanceId + "_" + this._autoId++;
    },

    _createItemModel: function (/*Object*/ item) {
      var modelClass = this.itemModelClass,
        id = { id: this._generateItemId() },
        model,
        dfd = new Deferred();

      if (this.getItemIndex(item) >= 0) {
        return item;
      }

      if (item instanceof modelClass) {
        model = lang.mixin(lang.clone(item), id);
      } else {
        model = new modelClass(lang.mixin(lang.clone(item), id));
      }

      when(model.parse(true), function () {
        dfd.resolve(model);
      });

      return dfd;
    },

    _move: function (direction) {
      var index = this.getItemIndex(this._selectedItem);
      if (index < 0) {
        return false;
      }

      return this.swap(index, index + direction);
    },

    _canMoveNextGetter: function () {
      var index = this.getItemIndex(this._selectedItem);
      return !!(
        this._selectedItem &&
        index >= 0 &&
        index < this._data.length - 1
      );
    },

    _canMovePreviousGetter: function () {
      var index = this.getItemIndex(this._selectedItem);
      return !!(
        this._selectedItem &&
        index > 0 &&
        index <= this._data.length - 1
      );
    },

    _canEditGetter: function () {
      return !this.readOnly && !!this._selectedItem;
    },

    _valueGetter: function () {
      return this._dataGetter().map(function (item) {
        return item.serialize();
      }, this);
    },

    _dataGetter: function () {
      return this._data || [];
    },

    _dataSetter: function (data) {
      this._init(data);
    },

    _selectedItemGetter: function () {
      return this._selectedItem;
    },

    _selectedItemSetter: function (value) {
      this._selectedItem = value;
    },
  });
});
