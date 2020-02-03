using System.Collections.Generic;

namespace IdentityServer.Api.IdentityServerAdmin.Dtos.ApiResources
{
    public class ApiSecretsApiDto
    {
        public ApiSecretsApiDto()
        {
            ApiSecrets = new List<ApiSecretApiDto>();
        }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public List<ApiSecretApiDto> ApiSecrets { get; set; }
    }
}





