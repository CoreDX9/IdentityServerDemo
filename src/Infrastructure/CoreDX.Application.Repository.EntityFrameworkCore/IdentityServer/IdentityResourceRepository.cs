using CoreDX.Common.Util.EnumerableExtensions;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Repository.App.IdentityServer;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Application.Repository.EntityFrameworkCore.IdentityServer
{
    public class IdentityResourceRepository<TDbContext> : IIdentityResourceRepository
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        protected readonly TDbContext DbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityResourceRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<IPagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            return await DbContext.IdentityResources
                .WhereIf(!search.IsNullOrEmpty(), x => x.Name.Contains(search))
                .OrderBy(x => x.Name)
                .ToPagedListAsync(page, pageSize);
        }

        public virtual Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId)
        {
            return DbContext.IdentityResources
                .Include(x => x.UserClaims)
                .Where(x => x.Id == identityResourceId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public virtual async Task<IPagedList<IdentityResourceProperty>> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
        {
            return await DbContext.IdentityResourceProperties
                .Where(x => x.IdentityResource.Id == identityResourceId)
                .OrderBy(x => x.Id)
                .ToPagedListAsync(page, pageSize);
        }

        public virtual Task<IdentityResourceProperty> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
        {
            return DbContext.IdentityResourceProperties
                .Include(x => x.IdentityResource)
                .Where(x => x.Id == identityResourcePropertyId)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<int> AddIdentityResourcePropertyAsync(int identityResourceId, IdentityResourceProperty identityResourceProperty)
        {
            var identityResource = await DbContext.IdentityResources.Where(x => x.Id == identityResourceId).SingleOrDefaultAsync();

            identityResourceProperty.IdentityResource = identityResource;
            await DbContext.IdentityResourceProperties.AddAsync(identityResourceProperty);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourceProperty identityResourceProperty)
        {
            var propertyToDelete = await DbContext.IdentityResourceProperties.Where(x => x.Id == identityResourceProperty.Id).SingleOrDefaultAsync();

            DbContext.IdentityResourceProperties.Remove(propertyToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourceProperty identityResourceProperty)
        {
            var existsWithSameName = await DbContext.IdentityResourceProperties.Where(x => x.Key == identityResourceProperty.Key
                                                                                       && x.IdentityResource.Id == identityResourceProperty.IdentityResourceId).SingleOrDefaultAsync();
            return existsWithSameName == null;
        }

        /// <summary>
        /// Add new identity resource
        /// </summary>
        /// <param name="identityResource"></param>
        /// <returns>This method return new identity resource id</returns>
        public virtual async Task<int> AddIdentityResourceAsync(IdentityResource identityResource)
        {
            DbContext.IdentityResources.Add(identityResource);

            await AutoSaveChangesAsync();

            return identityResource.Id;
        }

        public virtual async Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource)
        {
            if (identityResource.Id == 0)
            {
                var existsWithSameName = await DbContext.IdentityResources.Where(x => x.Name == identityResource.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await DbContext.IdentityResources.Where(x => x.Name == identityResource.Name && x.Id != identityResource.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        private async Task RemoveIdentityResourceClaimsAsync(IdentityResource identityResource)
        {
            var identityClaims = await DbContext.IdentityClaims.Where(x => x.IdentityResource.Id == identityResource.Id).ToListAsync();
            DbContext.IdentityClaims.RemoveRange(identityClaims);
        }

        public virtual async Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource)
        {
            var identityResourceToDelete = await DbContext.IdentityResources.Where(x => x.Id == identityResource.Id).SingleOrDefaultAsync();

            DbContext.IdentityResources.Remove(identityResourceToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource)
        {
            //Remove old relations
            await RemoveIdentityResourceClaimsAsync(identityResource);

            //Update with new data
            DbContext.IdentityResources.Update(identityResource);

            return await AutoSaveChangesAsync();
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }

    public enum SavedStatus
    {
        WillBeSavedExplicitly = 0
    }
}
