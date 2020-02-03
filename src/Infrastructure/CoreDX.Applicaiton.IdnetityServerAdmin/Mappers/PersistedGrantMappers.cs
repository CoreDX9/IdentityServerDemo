using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Mappers
{
    public static class PersistedGrantMappers
    {
        //static PersistedGrantMappers()
        //{
        //    Mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantMapperProfile>())
        //        .CreateMapper();
        //}

        //internal static IMapper Mapper { get; }

        public static PersistedGrantsDto ToModel(this PagedList<PersistedGrantDataView> grant, IMapper mapper)
        {
            return grant == null ? null : mapper.Map<PersistedGrantsDto>(grant);
        }

        public static PersistedGrantsDto ToModel(this PagedList<PersistedGrant> grant, IMapper mapper)
        {
            return grant == null ? null : mapper.Map<PersistedGrantsDto>(grant);
        }

        public static PersistedGrantDto ToModel(this PersistedGrant grant, IMapper mapper)
        {
            return grant == null ? null : mapper.Map<PersistedGrantDto>(grant);
        }
    }
}