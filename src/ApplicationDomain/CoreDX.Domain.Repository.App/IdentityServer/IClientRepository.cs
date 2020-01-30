using CoreDX.Domain.Repository.App.IdentityServer.Helpers;
using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Domain.Repository.App.IdentityServer
{
	public interface IClientRepository
	{
		Task<int> AddClientAsync(Client client);

		Task<int> UpdateClientAsync(Client client);

		Task<int> RemoveClientAsync(Client client);

		Task<int> CloneClientAsync(Client client,
			bool cloneClientCorsOrigins = true,
			bool cloneClientGrantTypes = true,
			bool cloneClientIdPRestrictions = true,
			bool cloneClientPostLogoutRedirectUris = true,
			bool cloneClientScopes = true,
			bool cloneClientRedirectUris = true,
			bool cloneClientClaims = true,
			bool cloneClientProperties = true);

		Task<bool> CanInsertClientAsync(Client client, bool isCloned = false);

		Task<Client> GetClientAsync(int clientId);

		Task<(string ClientId, string ClientName)> GetClientIdAsync(int clientId);

		Task<IPagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10);

		Task<List<string>> GetScopesAsync(string scope, int limit = 0);

		List<string> GetGrantTypes(string grant, int limit = 0);

		List<SelectItem> GetProtocolTypes();

		List<SelectItem> GetAccessTokenTypes();

		List<SelectItem> GetTokenExpirations();

		List<SelectItem> GetTokenUsage();

		List<SelectItem> GetHashTypes();

		List<SelectItem> GetSecretTypes();

		List<string> GetStandardClaims(string claim, int limit = 0);

		Task<int> AddClientSecretAsync(int clientId, ClientSecret clientSecret);

		Task<int> DeleteClientSecretAsync(ClientSecret clientSecret);

		Task<IPagedList<ClientSecret>> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10);

		Task<ClientSecret> GetClientSecretAsync(int clientSecretId);

		Task<IPagedList<ClientClaim>> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10);

		Task<IPagedList<ClientProperty>> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10);

		Task<ClientClaim> GetClientClaimAsync(int clientClaimId);

		Task<ClientProperty> GetClientPropertyAsync(int clientPropertyId);

		Task<int> AddClientClaimAsync(int clientId, ClientClaim clientClaim);

		Task<int> AddClientPropertyAsync(int clientId, ClientProperty clientProperties);

		Task<int> DeleteClientClaimAsync(ClientClaim clientClaim);

		Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty);

		Task<int> SaveAllChangesAsync();
	}
}
