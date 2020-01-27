using CoreDX.Domain.Entity.App.IdentityServer;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Domain.Service.App.IdentityServer
{
    public interface IApiResourceService
    {
        ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets);

        ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope);

        Task<IPagedList<ApiResourceDto>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId);

        Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties);

        Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

        Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

        Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId);

        Task<int> AddApiResourceAsync(ApiResourceDto apiResource);

        Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource);

        Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource);

        Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource);

        Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId);

        Task<int> AddApiScopeAsync(ApiScopesDto apiScope);

        Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope);

        Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope);

        Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret);

        Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId);

        Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret);

        Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes);

        Task<string> GetApiResourceNameAsync(int apiResourceId);
    }
}
