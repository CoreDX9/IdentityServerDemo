using AutoMapper;
using CoreDX.Domain.Entity.Identity;
using IdentityServer.Areas.Manage.Models.Users;

namespace IdentityServer.Areas.Manage.Models.ModelMapProfiles
{
    public class ApplicationUserMapProfile : Profile
    {
        public ApplicationUserMapProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();
            //映射发生之前
            //.BeforeMap((source, dto) => {
            //    //可以较为精确的控制输出数据格式
            //})
            ////映射发生之后
            //.AfterMap((source, dto) => {
            //    //code ...
            //});
        }
    }
}
