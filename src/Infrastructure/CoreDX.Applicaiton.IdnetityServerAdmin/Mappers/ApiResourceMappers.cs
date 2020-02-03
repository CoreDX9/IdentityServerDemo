using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Mappers
{
    public static class ApiResourceMappers
    {
        //static ApiResourceMappers()
        //{
        //    Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
        //        .CreateMapper();
        //}

        //internal static IMapper Mapper { get; }

        public static ApiResourceDto ToModel(this ApiResource resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<ApiResourceDto>(resource);
        }

        public static ApiResourcesDto ToModel(this PagedList<ApiResource> resources, IMapper mapper)
        {
            return resources == null ? null : mapper.Map<ApiResourcesDto>(resources);
        }

        public static ApiResourcePropertiesDto ToModel(this PagedList<ApiResourceProperty> apiResourceProperties, IMapper mapper)
        {
            return mapper.Map<ApiResourcePropertiesDto>(apiResourceProperties);
        }

        public static ApiResourcePropertiesDto ToModel(this ApiResourceProperty apiResourceProperty, IMapper mapper)
        {
            return mapper.Map<ApiResourcePropertiesDto>(apiResourceProperty);
        }

        public static ApiSecretsDto ToModel(this PagedList<ApiSecret> secrets, IMapper mapper)
        {
            return secrets == null ? null : mapper.Map<ApiSecretsDto>(secrets);
        }

        public static ApiScopesDto ToModel(this PagedList<ApiScope> scopes, IMapper mapper)
        {
            return scopes == null ? null : mapper.Map<ApiScopesDto>(scopes);
        }

        public static ApiScopesDto ToModel(this ApiScope resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<ApiScopesDto>(resource);
        }

        public static ApiSecretsDto ToModel(this ApiSecret resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<ApiSecretsDto>(resource);
        }

        public static ApiResource ToEntity(this ApiResourceDto resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<ApiResource>(resource);
        }

        public static ApiSecret ToEntity(this ApiSecretsDto resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<ApiSecret>(resource);
        }

        public static ApiScope ToEntity(this ApiScopesDto resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<ApiScope>(resource);
        }

        public static ApiResourceProperty ToEntity(this ApiResourcePropertiesDto apiResourceProperties, IMapper mapper)
        {
            return mapper.Map<ApiResourceProperty>(apiResourceProperties);
        }
    }
}