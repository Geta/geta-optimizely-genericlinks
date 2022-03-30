define("genericLinks/editors/GenericCollectionEditor", [
  "dojo/_base/declare",
  "epi-cms/contentediting/editors/ItemCollectionEditor",
  "genericLinks/viewmodel/ItemCollectionViewModel",
], function (declare, ItemCollectionEditor, ItemCollectionViewModel) {
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
  });
});
