using EPiServer;
using EPiServer.Authorization;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using Mediachase.Commerce.Catalog;

namespace AlloyMvcTemplates.Business.Initialization
{
    public class EnableCatalogRoot
    {
        private readonly IContentLoader _contentLoader;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentSecurityRepository _contentSecurityRepository;

        public EnableCatalogRoot(
            IContentLoader contentLoader,
            ReferenceConverter referenceConverter,
            IContentSecurityRepository contentSecurityRepository)
        {
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
            _contentSecurityRepository = contentSecurityRepository;
        }

        public void SetCatalogAccessRights()
        {
            if (_contentLoader.TryGet(_referenceConverter.GetRootLink(), out IContent content))
            {
                var contentSecurable = (IContentSecurable)content;
                var writableClone = (IContentSecurityDescriptor)contentSecurable.GetContentSecurityDescriptor().CreateWritableClone();
                writableClone.AddEntry(new AccessControlEntry(Roles.Administrators, AccessLevel.FullAccess, SecurityEntityType.Role));
                writableClone.AddEntry(new AccessControlEntry(Roles.WebAdmins, AccessLevel.FullAccess, SecurityEntityType.Role));
                writableClone.AddEntry(new AccessControlEntry(EveryoneRole.RoleName, AccessLevel.Read, SecurityEntityType.Role));

                _contentSecurityRepository.Save(content.ContentLink, writableClone, SecuritySaveType.Replace);
            }
        }
    }
}
