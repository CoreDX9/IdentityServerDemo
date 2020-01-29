using IdentityServer4.EntityFramework.Entities;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Domain.Repository.App.IdentityServer
{
    public interface IIdentityResourceRepository
    {
        Task<IPagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId);

        Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource);

        Task<int> AddIdentityResourceAsync(IdentityResource identityResource);

        Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource);

        Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource);

        Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourceProperty identityResourceProperty);

        Task<IPagedList<IdentityResourceProperty>> GetIdentityResourcePropertiesAsync(int identityResourceId,
            int page = 1, int pageSize = 10);

        Task<IdentityResourceProperty> GetIdentityResourcePropertyAsync(int identityResourcePropertyId);

        Task<int> AddIdentityResourcePropertyAsync(int identityResourceId,
            IdentityResourceProperty identityResourceProperty);

        Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourceProperty identityResourceProperty);

        Task<int> SaveAllChangesAsync();
    }
}
