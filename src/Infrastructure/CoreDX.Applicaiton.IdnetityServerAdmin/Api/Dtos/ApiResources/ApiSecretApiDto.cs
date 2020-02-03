using System;
using System.ComponentModel.DataAnnotations;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Api.Dtos.ApiResources
{
    public class ApiSecretApiDto
    {
        [Required]
        public string Type { get; set; } = "SharedSecret";

        public int Id { get; set; }

        public string Description { get; set; }

        [Required]
        public string Value { get; set; }

        public DateTime? Expiration { get; set; }
    }
}





