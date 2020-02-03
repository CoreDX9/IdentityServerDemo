using AutoMapper;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Api.Mappers
{
    public static class IdentityResourceApiMappers
    {
        //static IdentityResourceApiMappers()
        //{
        //    Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceApiMapperProfile>())
        //        .CreateMapper();
        //}

        //internal static IMapper Mapper { get; }

        public static T ToIdentityResourceApiModel<T>(this object source, IMapper mapper)
        {
            return mapper.Map<T>(source);
        }
    }
}





