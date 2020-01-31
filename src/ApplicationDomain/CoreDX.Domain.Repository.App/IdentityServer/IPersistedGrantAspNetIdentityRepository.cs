using CoreDX.Domain.Entity.App.IdentityServer;
using IdentityServer4.EntityFramework.Entities;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Domain.Repository.App.IdentityServer
{
	public interface IPersistedGrantAspNetIdentityRepository
	{
		Task<IPagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10);
		Task<IPagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10);
		Task<PersistedGrant> GetPersistedGrantAsync(string key);
		Task<int> DeletePersistedGrantAsync(string key);
		Task<int> DeletePersistedGrantsAsync(string userId);
		Task<bool> ExistsPersistedGrantsAsync(string subjectId);
		Task<bool> ExistsPersistedGrantAsync(string key);
		Task<int> SaveAllChangesAsync();
	}
}
