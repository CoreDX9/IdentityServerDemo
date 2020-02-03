namespace CoreDX.Applicaiton.IdnetityServerAdmin.Api.Dtos.Users
{
    public class UserProviderDeleteApiDto<TUserDtoKey>
    {
        public TUserDtoKey UserId { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }
    }
}





