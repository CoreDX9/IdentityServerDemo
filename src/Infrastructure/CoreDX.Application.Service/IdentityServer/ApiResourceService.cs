using AutoMapper;
using CoreDX.Application.Service.IdentityServer.Helpers;
using CoreDX.Common.Util.Security;
using CoreDX.Domain.Entity.App.IdentityServer;
using CoreDX.Domain.Repository.App.IdentityServer;
using CoreDX.Domain.Service.App.IdentityServer;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreDX.Application.Service.IdentityServer
{
    public class ApiResourceService : IApiResourceService
    {
        protected readonly IApiResourceRepository ApiResourceRepository;
        protected readonly IClientService ClientService;
        protected readonly IMapper _mapper;
        private const string SharedSecret = "SharedSecret";

        public ApiResourceService(IApiResourceRepository apiResourceRepository,
            IClientService clientService,
            IMapper mapper)
        {
            ApiResourceRepository = apiResourceRepository;
            ClientService = clientService;
            _mapper = mapper;
        }

        public virtual async Task<IPagedList<ApiResourceDto>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ApiResourceRepository.GetApiResourcesAsync(search, page, pageSize);
            var apiResourcesDto = pagedList.Select(x => _mapper.Map<ApiResourceDto>(x));

            return apiResourcesDto;
        }

        public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            //if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            if (apiResource == null) throw new Exception(string.Format("{0}"));

            var pagedList = await ApiResourceRepository.GetApiResourcePropertiesAsync(apiResourceId, page, pageSize);
            var apiResourcePropertiesDto = _mapper.Map<ApiResourcePropertiesDto>(pagedList);
            apiResourcePropertiesDto.ApiResourceId = apiResourceId;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiResourcePropertiesDto;
        }

        public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId)
        {
            var apiResourceProperty = await ApiResourceRepository.GetApiResourcePropertyAsync(apiResourcePropertyId);
            //if (apiResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourcePropertyDoesNotExist().Description, apiResourcePropertyId));
            if (apiResourceProperty == null) new Exception(string.Format("{0}"));

            var apiResourcePropertiesDto = _mapper.Map<ApiResourcePropertiesDto>(apiResourceProperty);
            apiResourcePropertiesDto.ApiResourceId = apiResourceProperty.ApiResourceId;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceProperty.ApiResourceId);

            return apiResourcePropertiesDto;
        }

        public virtual async Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var canInsert = await CanInsertApiResourcePropertyAsync(apiResourceProperties);
            if (!canInsert)
            {
                await BuildApiResourcePropertiesViewModelAsync(apiResourceProperties);
                //throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourcePropertyExistsValue().Description, apiResourceProperties.Key), ApiResourceServiceResources.ApiResourcePropertyExistsKey().Description, apiResourceProperties);
                new Exception(string.Format("{0}"));
            }

            var apiResourceProperty = _mapper.Map<ApiResourceProperty>(apiResourceProperties);

            var saved = await ApiResourceRepository.AddApiResourcePropertyAsync(apiResourceProperties.ApiResourceId, apiResourceProperty);

            return saved;
        }

        public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var propertyEntity = _mapper.Map<ApiResourceProperty>(apiResourceProperty);

            var deleted = await ApiResourceRepository.DeleteApiResourcePropertyAsync(propertyEntity);

            return deleted;
        }

        public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var resource = _mapper.Map<ApiResourceProperty>(apiResourceProperty);

            return await ApiResourceRepository.CanInsertApiResourcePropertyAsync(resource);
        }

        private void HashApiSharedSecret(ApiSecretsDto apiSecret)
        {
            if (apiSecret.Type != SharedSecret) return;

            if (apiSecret.HashType == ((int)HashType.Sha256).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha256();
            }
            else if (apiSecret.HashType == ((int)HashType.Sha512).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha512();
            }
        }

        public virtual ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets)
        {
            apiSecrets.HashTypes = ClientService.GetHashTypes();
            apiSecrets.TypeList = ClientService.GetSecretTypes();

            return apiSecrets;
        }

        public virtual async Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            //if (apiResource == null) throw new UserFriendlyErrorPageException(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            if (apiResource == null) new Exception(string.Format("{0}"));

            var apiResourceDto = _mapper.Map<ApiResourceDto>(apiResource);

            return apiResourceDto;
        }

        public virtual async Task<int> AddApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                //throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
                new Exception(string.Format("{0}"));
            }

            var resource = _mapper.Map<ApiResource>(apiResource);

            var added = await ApiResourceRepository.AddApiResourceAsync(resource);

            return added;
        }

        public virtual async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                //throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
                new Exception(string.Format("{0}"));
            }

            var resource = _mapper.Map<ApiResource>(apiResource);

            var originalApiResource = await GetApiResourceAsync(apiResource.Id);

            var updated = await ApiResourceRepository.UpdateApiResourceAsync(resource);

            return updated;
        }

        public virtual async Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = _mapper.Map<ApiResource>(apiResource);

            var deleted = await ApiResourceRepository.DeleteApiResourceAsync(resource);

            return deleted;
        }

        public virtual async Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = _mapper.Map<ApiResource>(apiResource);

            return await ApiResourceRepository.CanInsertApiResourceAsync(resource);
        }

        public virtual async Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            //if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            if (apiResource == null) new Exception(string.Format("{0}"));

            var pagedList = await ApiResourceRepository.GetApiScopesAsync(apiResourceId, page, pageSize);

            var apiScopesDto = _mapper.Map<ApiScopesDto>(pagedList);
            apiScopesDto.ApiResourceId = apiResourceId;
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public virtual async Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            //if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            if (apiResource == null) new Exception(string.Format("{0}"));

            var apiScope = await ApiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);
            //if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), ApiResourceServiceResources.ApiScopeDoesNotExist().Description);
            if (apiScope == null) new Exception(string.Format("{0}"));

            var apiScopesDto = _mapper.Map<ApiScopesDto>(apiScope);
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public virtual async Task<int> AddApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                //throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
                new Exception(string.Format("{0}"));
            }

            var scope = _mapper.Map<ApiScope>(apiScope);

            var added = await ApiResourceRepository.AddApiScopeAsync(apiScope.ApiResourceId, scope);

            return added;
        }

        public virtual ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope)
        {
            ComboBoxHelpers.PopulateValuesToList(apiScope.UserClaimsItems, apiScope.UserClaims);

            return apiScope;
        }

        private async Task BuildApiScopesViewModelAsync(ApiScopesDto apiScope)
        {
            if (apiScope.ApiScopeId == 0)
            {
                var apiScopesDto = await GetApiScopesAsync(apiScope.ApiResourceId);
                apiScope.Scopes.AddRange(apiScopesDto.Scopes);
                apiScope.TotalCount = apiScopesDto.TotalCount;
            }
        }

        private async Task BuildApiResourcePropertiesViewModelAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var apiResourcePropertiesDto = await GetApiResourcePropertiesAsync(apiResourceProperties.ApiResourceId);
            apiResourceProperties.ApiResourceProperties.AddRange(apiResourcePropertiesDto.ApiResourceProperties);
            apiResourceProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
        }

        public virtual async Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                //throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
                new Exception(string.Format("{0}"));
            }

            var scope = _mapper.Map<ApiScope>(apiScope);

            var originalApiScope = await GetApiScopeAsync(apiScope.ApiResourceId, apiScope.ApiScopeId);

            var updated = await ApiResourceRepository.UpdateApiScopeAsync(apiScope.ApiResourceId, scope);

            return updated;
        }

        public virtual async Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope)
        {
            var scope = _mapper.Map<ApiScope>(apiScope);

            var deleted = await ApiResourceRepository.DeleteApiScopeAsync(scope);

            return deleted;
        }

        public virtual async Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            //if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            if (apiResource == null) new Exception(string.Format("{0}"));

            var pagedList = await ApiResourceRepository.GetApiSecretsAsync(apiResourceId, page, pageSize);

            var apiSecretsDto = _mapper.Map<ApiSecretsDto>(pagedList);
            apiSecretsDto.ApiResourceId = apiResourceId;
            apiSecretsDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiSecretsDto;
        }

        public virtual async Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret)
        {
            HashApiSharedSecret(apiSecret);

            var secret = _mapper.Map<ApiSecret>(apiSecret);

            var added = await ApiResourceRepository.AddApiSecretAsync(apiSecret.ApiResourceId, secret);

            return added;
        }

        public virtual async Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId)
        {
            var apiSecret = await ApiResourceRepository.GetApiSecretAsync(apiSecretId);
            //if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), ApiResourceServiceResources.ApiSecretDoesNotExist().Description);
            if (apiSecret == null) new Exception(string.Format("{0}"));
            var apiSecretsDto = _mapper.Map<ApiSecretsDto>(apiSecret);

            return apiSecretsDto;
        }

        public virtual async Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret)
        {
            var secret = _mapper.Map<ApiSecret>(apiSecret);

            var deleted = await ApiResourceRepository.DeleteApiSecretAsync(secret);

            return deleted;
        }

        public virtual async Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes)
        {
            var apiScope = _mapper.Map<ApiScope>(apiScopes);

            return await ApiResourceRepository.CanInsertApiScopeAsync(apiScope);
        }

        public virtual async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);
        }
    }
}
