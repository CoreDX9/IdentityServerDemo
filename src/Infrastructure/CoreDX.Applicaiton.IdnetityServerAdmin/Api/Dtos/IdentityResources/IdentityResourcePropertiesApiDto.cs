using System.Collections.Generic;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Api.Dtos.IdentityResources
{
    public class IdentityResourcePropertiesApiDto
    {
        public IdentityResourcePropertiesApiDto()
        {
            IdentityResourceProperties = new List<IdentityResourcePropertyApiDto>();
        }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public List<IdentityResourcePropertyApiDto> IdentityResourceProperties { get; set; }
    }
}





