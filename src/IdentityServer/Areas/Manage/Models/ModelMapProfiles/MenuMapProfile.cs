using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Management;
using IdentityServer.Areas.Manage.Models.Menus;

namespace IdentityServer.Areas.Manage.Models.ModelMapProfiles
{
    public class MenuMapProfile : Profile
    {
        public MenuMapProfile()
        {
            CreateMap<Menu, MenuViewModel>()
                .ForMember(mvm => mvm.Index, opts => opts.MapFrom(src => src.Id.ToString()));
        }
    }

    public class MenuItemMapProfile : Profile
    {
        public MenuItemMapProfile()
        {
            CreateMap<MenuItem, MenuItemViewModel>()
                .ForMember(mivm => mivm.Index, opts => opts.MapFrom(src => src.Id.ToString()));
        }
    }
}
