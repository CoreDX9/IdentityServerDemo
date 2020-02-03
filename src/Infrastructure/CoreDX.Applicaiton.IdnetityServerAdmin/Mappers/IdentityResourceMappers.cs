using System.Collections.Generic;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Mappers
{
    public static class IdentityResourceMappers
    {
        //static IdentityResourceMappers()
        //{
        //    Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
        //        .CreateMapper();
        //}

        //internal static IMapper Mapper { get; }


        public static IdentityResourceDto ToModel(this IdentityResource resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<IdentityResourceDto>(resource);
        }

        public static IdentityResourcesDto ToModel(this PagedList<IdentityResource> resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<IdentityResourcesDto>(resource);
        }

        public static List<IdentityResourceDto> ToModel(this List<IdentityResource> resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<List<IdentityResourceDto>>(resource);
        }

        public static IdentityResource ToEntity(this IdentityResourceDto resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<IdentityResource>(resource);
        }

        public static IdentityResourcePropertiesDto ToModel(this PagedList<IdentityResourceProperty> identityResourceProperties, IMapper mapper)
        {
            return mapper.Map<IdentityResourcePropertiesDto>(identityResourceProperties);
        }

        public static IdentityResourcePropertiesDto ToModel(this IdentityResourceProperty identityResourceProperty, IMapper mapper)
        {
            return mapper.Map<IdentityResourcePropertiesDto>(identityResourceProperty);
        }

        public static List<IdentityResource> ToEntity(this List<IdentityResourceDto> resource, IMapper mapper)
        {
            return resource == null ? null : mapper.Map<List<IdentityResource>>(resource);
        }

        public static IdentityResourceProperty ToEntity(this IdentityResourcePropertiesDto identityResourceProperties, IMapper mapper)
        {
            return mapper.Map<IdentityResourceProperty>(identityResourceProperties);
        }
    }
}