using System.Collections.Generic;

namespace IdentityServer.Api.IdentityServerAdmin.Dtos.Roles
{
    public class RoleClaimsApiDto<TRoleDtoKey>
    {
        public RoleClaimsApiDto()
        {
            Claims = new List<RoleClaimApiDto<TRoleDtoKey>>();
        }

        public List<RoleClaimApiDto<TRoleDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}





