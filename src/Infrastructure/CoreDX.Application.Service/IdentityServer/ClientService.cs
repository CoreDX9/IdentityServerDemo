using AutoMapper;
using CoreDX.Application.Service.IdentityServer.Helpers;
using CoreDX.Common.Util.Security;
using CoreDX.Domain.Entity.App.IdentityServer;
using CoreDX.Domain.Repository.App.IdentityServer;
using CoreDX.Domain.Repository.App.IdentityServer.Helpers;
using CoreDX.Domain.Service.App.IdentityServer;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Application.Service.IdentityServer
{
    public class ClientService : IClientService
    {
        protected readonly IClientRepository ClientRepository;
        protected readonly IMapper _mapper;
        private const string SharedSecret = "SharedSecret";

        public ClientService(IClientRepository clientRepository, IMapper mapper)
        {
            ClientRepository = clientRepository;
            _mapper = mapper;
        }

        private void HashClientSharedSecret(ClientSecretsDto clientSecret)
        {
            if (clientSecret.Type != SharedSecret) return;

            if (clientSecret.HashType == ((int)HashType.Sha256).ToString())
            {
                clientSecret.Value = clientSecret.Value.Sha256();
            }
            else if (clientSecret.HashType == ((int)HashType.Sha512).ToString())
            {
                clientSecret.Value = clientSecret.Value.Sha512();
            }
        }

        private void PrepareClientTypeForNewClient(ClientDto client)
        {
            switch (client.ClientType)
            {
                case ClientType.Empty:
                    break;
                case ClientType.WebHybrid:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Hybrid);
                    break;
                case ClientType.Spa:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
                    break;
                case ClientType.Native:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Hybrid);
                    break;
                case ClientType.Machine:
                    client.AllowedGrantTypes.AddRange(GrantTypes.ResourceOwnerPasswordAndClientCredentials);
                    break;
                case ClientType.Device:
                    client.AllowedGrantTypes.AddRange(GrantTypes.DeviceFlow);
                    client.RequireClientSecret = false;
                    client.AllowOfflineAccess = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PopulateClientRelations(ClientDto client)
        {
            ComboBoxHelpers.PopulateValuesToList(client.AllowedScopesItems, client.AllowedScopes);
            ComboBoxHelpers.PopulateValuesToList(client.PostLogoutRedirectUrisItems, client.PostLogoutRedirectUris);
            ComboBoxHelpers.PopulateValuesToList(client.IdentityProviderRestrictionsItems, client.IdentityProviderRestrictions);
            ComboBoxHelpers.PopulateValuesToList(client.RedirectUrisItems, client.RedirectUris);
            ComboBoxHelpers.PopulateValuesToList(client.AllowedCorsOriginsItems, client.AllowedCorsOrigins);
            ComboBoxHelpers.PopulateValuesToList(client.AllowedGrantTypesItems, client.AllowedGrantTypes);
        }

        public virtual ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto)
        {
            var client = new ClientCloneDto
            {
                CloneClientCorsOrigins = true,
                CloneClientGrantTypes = true,
                CloneClientIdPRestrictions = true,
                CloneClientPostLogoutRedirectUris = true,
                CloneClientRedirectUris = true,
                CloneClientScopes = true,
                CloneClientClaims = true,
                CloneClientProperties = true,
                ClientIdOriginal = clientDto.ClientId,
                ClientNameOriginal = clientDto.ClientName,
                Id = id
            };

            return client;
        }

        public virtual ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets)
        {
            clientSecrets.HashTypes = GetHashTypes();
            clientSecrets.TypeList = GetSecretTypes();

            return clientSecrets;
        }

        public virtual ClientDto BuildClientViewModel(ClientDto client = null)
        {
            if (client == null)
            {
                var clientDto = new ClientDto
                {
                    AccessTokenTypes = GetAccessTokenTypes(),
                    RefreshTokenExpirations = GetTokenExpirations(),
                    RefreshTokenUsages = GetTokenUsage(),
                    ProtocolTypes = GetProtocolTypes(),
                    Id = 0
                };

                return clientDto;
            }

            client.AccessTokenTypes = GetAccessTokenTypes();
            client.RefreshTokenExpirations = GetTokenExpirations();
            client.RefreshTokenUsages = GetTokenUsage();
            client.ProtocolTypes = GetProtocolTypes();

            PopulateClientRelations(client);

            return client;
        }

        /// <summary>
        /// Add new client, this method doesn't save client secrets, client claims, client properties
        /// </summary>
        /// <param name="client"></param>
        /// <returns>This method return new client id</returns>
        public virtual async Task<int> AddClientAsync(ClientDto client)
        {
            var canInsert = await CanInsertClientAsync(client);
            if (!canInsert)
            {
                //throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
                throw new Exception(string.Format("{0}"));
            }

            PrepareClientTypeForNewClient(client);
            var clientEntity = _mapper.Map<Client>(client);

            var added = await ClientRepository.AddClientAsync(clientEntity);

            return added;
        }

        public virtual async Task<int> UpdateClientAsync(ClientDto client)
        {
            var canInsert = await CanInsertClientAsync(client);
            if (!canInsert)
            {
                //throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
                throw new Exception(string.Format("{0}"));
            }

            var clientEntity = _mapper.Map<Client>(client);

            var originalClient = await GetClientAsync(client.Id);

            var updated = await ClientRepository.UpdateClientAsync(clientEntity);

            return updated;
        }

        public virtual async Task<int> RemoveClientAsync(ClientDto client)
        {
            var clientEntity = _mapper.Map<Client>(client);

            var deleted = await ClientRepository.RemoveClientAsync(clientEntity);

            return deleted;
        }

        public virtual async Task<int> CloneClientAsync(ClientCloneDto client)
        {
            var canInsert = await CanInsertClientAsync(client, true);
            if (!canInsert)
            {
                //If it failed you need get original clientid, clientname for view title
                var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(client.Id);
                client.ClientIdOriginal = ClientId;
                client.ClientNameOriginal = ClientName;

                //throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
                throw new Exception(string.Format("{0}"));
            }

            var clientEntity = _mapper.Map<Client>(client);

            var clonedClientId = await ClientRepository.CloneClientAsync(clientEntity, client.CloneClientCorsOrigins,
                client.CloneClientGrantTypes, client.CloneClientIdPRestrictions,
                client.CloneClientPostLogoutRedirectUris,
                client.CloneClientScopes, client.CloneClientRedirectUris, client.CloneClientClaims, client.CloneClientProperties);

            return clonedClientId;
        }

        public virtual Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false)
        {
            var clientEntity = _mapper.Map<Client>(client);

            return ClientRepository.CanInsertClientAsync(clientEntity, isCloned);
        }

        public virtual async Task<ClientDto> GetClientAsync(int clientId)
        {
            var client = await ClientRepository.GetClientAsync(clientId);

            //if (client == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));
            if (client == null) throw new Exception(string.Format("{0}"));

            var clientDto = _mapper.Map<ClientDto>(client);

            return clientDto;
        }

        public virtual async Task<IPagedList<ClientDto>> GetClientsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ClientRepository.GetClientsAsync(search, page, pageSize);
            var clientsDto = pagedList.Select(x => _mapper.Map<ClientDto>(x));

            return clientsDto;
        }

        public virtual async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            var scopes = await ClientRepository.GetScopesAsync(scope, limit);

            return scopes;
        }

        public virtual List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var grantTypes = ClientRepository.GetGrantTypes(grant, limit);

            return grantTypes;
        }

        public virtual List<SelectItemDto> GetAccessTokenTypes()
        {
            var accessTokenTypes = _mapper.Map<List<SelectItemDto>>(ClientRepository.GetAccessTokenTypes());

            return accessTokenTypes;
        }

        public virtual List<SelectItemDto> GetTokenExpirations()
        {
            var tokenExpirations = _mapper.Map<List<SelectItemDto>>(ClientRepository.GetTokenExpirations());

            return tokenExpirations;
        }

        public virtual List<SelectItemDto> GetTokenUsage()
        {
            var tokenUsage = _mapper.Map<List<SelectItemDto>>(ClientRepository.GetTokenUsage());

            return tokenUsage;
        }

        public virtual List<SelectItemDto> GetHashTypes()
        {
            var hashTypes = _mapper.Map<List<SelectItemDto>>(ClientRepository.GetHashTypes());

            return hashTypes;
        }

        public virtual List<SelectItemDto> GetSecretTypes()
        {
            var secretTypes = _mapper.Map<List<SelectItemDto>>(ClientRepository.GetSecretTypes());

            return secretTypes;
        }

        public virtual List<SelectItemDto> GetProtocolTypes()
        {
            var protocolTypes = _mapper.Map<List<SelectItemDto>>(ClientRepository.GetProtocolTypes());

            return protocolTypes;
        }

        public virtual List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var standardClaims = ClientRepository.GetStandardClaims(claim, limit);

            return standardClaims;
        }

        public virtual async Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret)
        {
            HashClientSharedSecret(clientSecret);

            var clientSecretEntity = _mapper.Map<ClientSecret>(clientSecret);
            var added = await ClientRepository.AddClientSecretAsync(clientSecret.ClientId, clientSecretEntity);

            return added;
        }

        public virtual async Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret)
        {
            var clientSecretEntity = _mapper.Map<ClientSecret>(clientSecret);

            var deleted = await ClientRepository.DeleteClientSecretAsync(clientSecretEntity);

            return deleted;
        }

        public virtual async Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(clientId);
            //if (ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));
            if (ClientId == null) throw new Exception(string.Format("{0}"));

            var pagedList = await ClientRepository.GetClientSecretsAsync(clientId, page, pageSize);
            var clientSecretsDto = _mapper.Map<ClientSecretsDto>(pagedList);
            clientSecretsDto.ClientId = clientId;
            clientSecretsDto.ClientName = $"{ClientId} ({ClientName})";

            return clientSecretsDto;
        }

        public virtual async Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId)
        {
            var clientSecret = await ClientRepository.GetClientSecretAsync(clientSecretId);
            //if (clientSecret == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientSecretDoesNotExist().Description, clientSecretId));
            if (clientSecret == null) throw new Exception(string.Format("{0}"));

            var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(clientSecret.Client.Id);
            //if (ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientSecret.Client.Id));
            if (ClientId == null) throw new Exception(string.Format("{0}"));

            var clientSecretsDto = _mapper.Map<ClientSecretsDto>(clientSecret);
            clientSecretsDto.ClientId = clientSecret.Client.Id;
            clientSecretsDto.ClientName = $"{ClientId} ({ClientName})";

            return clientSecretsDto;
        }

        public virtual async Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(clientId);
            //if (ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));
            if (ClientId == null) throw new Exception(string.Format("{0}"));

            var pagedList = await ClientRepository.GetClientClaimsAsync(clientId, page, pageSize);
            var clientClaimsDto = _mapper.Map<ClientClaimsDto>(pagedList);
            clientClaimsDto.ClientId = clientId;
            clientClaimsDto.ClientName = $"{ClientId} ({ClientName})";

            return clientClaimsDto;
        }

        public virtual async Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(clientId);
            //if (ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));
            if (ClientId == null) throw new Exception(string.Format("{0}"));

            var pagedList = await ClientRepository.GetClientPropertiesAsync(clientId, page, pageSize);
            var clientPropertiesDto = _mapper.Map<ClientPropertiesDto>(pagedList);
            clientPropertiesDto.ClientId = clientId;
            clientPropertiesDto.ClientName = $"{ClientId} ({ClientName})";

            return clientPropertiesDto;
        }

        public virtual async Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId)
        {
            var clientClaim = await ClientRepository.GetClientClaimAsync(clientClaimId);
            //if (clientClaim == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientClaimDoesNotExist().Description, clientClaimId));
            if (clientClaim == null) throw new Exception(string.Format("{0}"));

            var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(clientClaim.Client.Id);
            //if (ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientClaim.Client.Id));
            if (ClientId == null) throw new Exception(string.Format("{0}"));

            var clientClaimsDto = _mapper.Map<ClientClaimsDto>(clientClaim);
            clientClaimsDto.ClientId = clientClaim.Client.Id;
            clientClaimsDto.ClientName = $"{ClientId} ({ClientName})";

            return clientClaimsDto;
        }

        public virtual async Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId)
        {
            var clientProperty = await ClientRepository.GetClientPropertyAsync(clientPropertyId);
            //if (clientProperty == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientPropertyDoesNotExist().Description, clientPropertyId));
            if (clientProperty == null) throw new Exception(string.Format("{0}"));

            var (ClientId, ClientName) = await ClientRepository.GetClientIdAsync(clientProperty.Client.Id);
            //if (ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientProperty.Client.Id));
            if (ClientId == null) throw new Exception(string.Format("{0}"));

            var clientPropertiesDto = _mapper.Map<ClientPropertiesDto>(clientProperty);
            clientPropertiesDto.ClientId = clientProperty.Client.Id;
            clientPropertiesDto.ClientName = $"{ClientId} ({ClientName})";

            return clientPropertiesDto;
        }

        public virtual async Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim)
        {
            var clientClaimEntity = _mapper.Map<ClientClaim>(clientClaim);

            var saved = await ClientRepository.AddClientClaimAsync(clientClaim.ClientId, clientClaimEntity);

            return saved;
        }

        public virtual async Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties)
        {
            var clientProperty = _mapper.Map<ClientProperty>(clientProperties);

            var saved = await ClientRepository.AddClientPropertyAsync(clientProperties.ClientId, clientProperty);

            return saved;
        }

        public virtual async Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim)
        {
            var clientClaimEntity = _mapper.Map<ClientClaim>(clientClaim);

            var deleted = await ClientRepository.DeleteClientClaimAsync(clientClaimEntity);

            return deleted;
        }

        public virtual async Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty)
        {
            var clientPropertyEntity = _mapper.Map<ClientProperty>(clientProperty);

            var deleted = await ClientRepository.DeleteClientPropertyAsync(clientPropertyEntity);

            return deleted;
        }
    }
}
