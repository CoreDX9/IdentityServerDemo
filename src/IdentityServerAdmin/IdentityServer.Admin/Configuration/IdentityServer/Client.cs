using System.Collections.Generic;
using IdentityServer.Admin.Configuration.Identity;

namespace IdentityServer.Admin.Configuration.IdentityServer
{
    public class Client : global::IdentityServer4.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}






