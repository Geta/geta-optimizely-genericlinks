define("genericLinks/viewmodel/LinkItemModel", [
  // Dojo
  "dojo/_base/declare",
  "dojo/_base/lang",
  "dojo/Deferred",
  "dojo/when",
  // EPi Framework
  "epi/Url",
  // EPi CMS
  "epi-cms/contentediting/viewmodel/CollectionItemModel",
  "epi-cms/core/PermanentLinkHelper",
], function (
  declare,
  lang,
  Deferred,
  when,
  Url,
  CollectionItemModel,
  PermanentLinkHelper
) {
  return declare([CollectionItemModel], {
    titleKey: "publicUrl",
    publicUrl: null,
    permanentUrl: null,

    _defaultDataStoreName: "epi.cms.content.light",

    serialize: function () {
      const {
        id,
        titleKey,
        text,
        href,
        url,
        target,
        title,
        publicUrl,
        previewUrl,
        permanentUrl,
        typeIdentifier,
        referencedPermanentLinkIds,
        attributes,
        ...rest
      } = this;
      const serialized = {
        text,
        href: href || url,
        target: target || null,
        title: title || "",
        publicUrl,
        typeIdentifier: typeIdentifier || "",
        attributes: { ...attributes, ...rest },
      };
      return serialized;
    },

    _buildPublicUrl: function (/*String*/ url) {
      var returnUrl = new Url(url);
      if (!returnUrl.scheme) {
        returnUrl = new Url("", {
          scheme: window.location.protocol,
          authority: window.location.hostname,
          path: url,
        });
      }
      return returnUrl.toString();
    },

    _publicUrlGetter: function () {
      return this._buildPublicUrl(this.publicUrl || this.url || this.href);
    },

    _permanentUrlSetter: function (/*String*/ value) {
      this.href = this.permanentUrl = value;
    },

    _onTryUpdateItemModel: function () {
      if (!this.typeIdentifier) {
        return when(
          PermanentLinkHelper.getContent(this.href),
          lang.hitch(this, function (content) {
            if (content) {
              this._parseFromContent(content);
            }
          })
        );
      }
    },

    _parseFromContent: function (content) {
      if (content) {
        this.typeIdentifier = content.typeIdentifier;
        this.publicUrl = this._buildPublicUrl(content.publicUrl);
      }
    },
  });
});
