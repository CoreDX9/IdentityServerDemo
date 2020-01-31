using CoreDX.Domain.Entity.App.IdentityServer;
using CoreDX.Domain.Repository.App.IdentityServer;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Application.Repository.EntityFrameworkCore.IdentityServer
{
    public class PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IPersistedGrantAspNetIdentityRepository
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
    {
        protected readonly TIdentityDbContext IdentityDbContext;
        protected readonly TPersistedGrantDbContext PersistedGrantDbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public PersistedGrantAspNetIdentityRepository(TIdentityDbContext identityDbContext, TPersistedGrantDbContext persistedGrantDbContext)
        {
            IdentityDbContext = identityDbContext;
            PersistedGrantDbContext = persistedGrantDbContext;
        }

        public virtual async Task<IPagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            static TResult Convert<TResult>(string value)
            {
                if (typeof(TResult) == typeof(string)) return (TResult)(object)value;
                if (typeof(TResult) == typeof(int)) return (TResult)(object)int.Parse(value);
                if (typeof(TResult) == typeof(long)) return (TResult)(object)long.Parse(value);
                if (typeof(TResult) == typeof(Guid)) return (TResult)(object)new Guid(value);

                throw new NotImplementedException($"不支持转换为 {nameof(TResult)}");
            }

            var persistedGrants = await PersistedGrantDbContext.PersistedGrants.AsNoTracking().ToListAsync();
            var userIds = persistedGrants.Select(x => Convert<TKey>(x.SubjectId)).ToArray();
            var users = await IdentityDbContext.Users.AsNoTracking().Where(x => userIds.Contains(x.Id)).Select(x => new { x.Id, x.UserName }).ToListAsync();
            var persistedGrantDataViews =
                (from pe in persistedGrants
                 join us in users on pe.SubjectId equals us.Id.ToString() into per
                 from identity in per.DefaultIfEmpty()
                 select new PersistedGrantDataView
                 {
                     SubjectId = pe.SubjectId,
                     SubjectName = identity == null ? string.Empty : identity.UserName
                 }).GroupBy(x => x.SubjectId).Select(g => g.First());

            if (!string.IsNullOrEmpty(search))
            {
                persistedGrantDataViews = persistedGrantDataViews.Where(x => x.SubjectId.Contains(search) || x.SubjectName.Contains(search));
            }

            var persistedGrantsData = await persistedGrantDataViews.OrderByDescending(x => x.SubjectId).ToPagedListAsync(page, pageSize);

            return persistedGrantsData;
        }

        public virtual async Task<IPagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
        {
            var persistedGrantsData = await PersistedGrantDbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).Select(x => new PersistedGrant()
            {
                SubjectId = x.SubjectId,
                Type = x.Type,
                Key = x.Key,
                ClientId = x.ClientId,
                Data = x.Data,
                Expiration = x.Expiration,
                CreationTime = x.CreationTime
            }).OrderByDescending(x => x.SubjectId).ToPagedListAsync(page, pageSize);

            return persistedGrantsData;
        }

        public virtual Task<PersistedGrant> GetPersistedGrantAsync(string key)
        {
            return PersistedGrantDbContext.PersistedGrants.SingleOrDefaultAsync(x => x.Key == key);
        }

        public virtual async Task<int> DeletePersistedGrantAsync(string key)
        {
            var persistedGrant = await PersistedGrantDbContext.PersistedGrants.Where(x => x.Key == key).SingleOrDefaultAsync();

            PersistedGrantDbContext.PersistedGrants.Remove(persistedGrant);

            return await AutoSaveChangesAsync();
        }

        public virtual Task<bool> ExistsPersistedGrantsAsync(string subjectId)
        {
            return PersistedGrantDbContext.PersistedGrants.AnyAsync(x => x.SubjectId == subjectId);
        }

        public Task<bool> ExistsPersistedGrantAsync(string key)
        {
            return PersistedGrantDbContext.PersistedGrants.AnyAsync(x => x.Key == key);
        }

        public virtual async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var grants = await PersistedGrantDbContext.PersistedGrants.Where(x => x.SubjectId == userId).ToListAsync();

            PersistedGrantDbContext.RemoveRange(grants);

            return await AutoSaveChangesAsync();
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await PersistedGrantDbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await PersistedGrantDbContext.SaveChangesAsync();
        }
    }
}
