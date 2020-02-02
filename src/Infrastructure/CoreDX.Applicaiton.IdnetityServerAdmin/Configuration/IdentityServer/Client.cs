using System.Collections.Generic;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Identity;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.IdentityServer
{
    public class Client : IdentityServer4.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}






