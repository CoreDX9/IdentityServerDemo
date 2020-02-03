using System.Collections.Generic;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Mappers
{
    public static class ClientMappers
    {
        //    static ClientMappers()
        //    {
        //        Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
        //            .CreateMapper();
        //    }

        //    internal static IMapper Mapper { get; }

        public static ClientDto ToModel(this Client client, IMapper mapper)
    {
            return mapper.Map<ClientDto>(client);
        }

        public static ClientSecretsDto ToModel(this PagedList<ClientSecret> clientSecret, IMapper mapper)
    {
            return mapper.Map<ClientSecretsDto>(clientSecret);
        }

        public static ClientClaimsDto ToModel(this PagedList<ClientClaim> clientClaims, IMapper mapper)
    {
            return mapper.Map<ClientClaimsDto>(clientClaims);
        }

        public static ClientsDto ToModel(this PagedList<Client> clients, IMapper mapper)
    {
            return mapper.Map<ClientsDto>(clients);
        }

        public static ClientPropertiesDto ToModel(this PagedList<ClientProperty> clientProperties, IMapper mapper)
    {
            return mapper.Map<ClientPropertiesDto>(clientProperties);
        }

        public static Client ToEntity(this ClientDto client, IMapper mapper)
    {
            return mapper.Map<Client>(client);
        }

        public static ClientSecretsDto ToModel(this ClientSecret clientSecret, IMapper mapper)
    {
            return mapper.Map<ClientSecretsDto>(clientSecret);
        }

        public static ClientSecret ToEntity(this ClientSecretsDto clientSecret, IMapper mapper)
    {
            return mapper.Map<ClientSecret>(clientSecret);
        }

        public static ClientClaimsDto ToModel(this ClientClaim clientClaim, IMapper mapper)
    {
            return mapper.Map<ClientClaimsDto>(clientClaim);
        }

        public static ClientPropertiesDto ToModel(this ClientProperty clientProperty, IMapper mapper)
    {
            return mapper.Map<ClientPropertiesDto>(clientProperty);
        }

        public static ClientClaim ToEntity(this ClientClaimsDto clientClaim, IMapper mapper)
    {
            return mapper.Map<ClientClaim>(clientClaim);
        }

        public static ClientProperty ToEntity(this ClientPropertiesDto clientProperties, IMapper mapper)
    {
            return mapper.Map<ClientProperty>(clientProperties);
        }

        public static SelectItemDto ToModel(this SelectItem selectItem, IMapper mapper)
    {
            return mapper.Map<SelectItemDto>(selectItem);
        }

        public static List<SelectItemDto> ToModel(this List<SelectItem> selectItem, IMapper mapper)
    {
            return mapper.Map<List<SelectItemDto>>(selectItem);
        }
    }
}