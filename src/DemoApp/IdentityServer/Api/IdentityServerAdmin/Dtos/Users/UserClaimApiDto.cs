using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Api.IdentityServerAdmin.Dtos.Users
{
    public class UserClaimApiDto<TUserDtoKey>
    {
        public int ClaimId { get; set; }

        public TUserDtoKey UserId { get; set; }

        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}





