using AutoMapper;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Api.Mappers
{
    public static class ClientApiMappers
    {
        //static ClientApiMappers()
        //{
        //    Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientApiMapperProfile>())
        //        .CreateMapper();
        //}

        //internal static IMapper Mapper { get; }

        public static T ToClientApiModel<T>(this object source, IMapper mapper)
        {
            return mapper.Map<T>(source);
        }
    }
}





