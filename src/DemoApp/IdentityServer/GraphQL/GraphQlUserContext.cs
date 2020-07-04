using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.GraphQL
{
    public class GraphQlUserContext : Dictionary<string, object>
    {
        public ClaimsPrincipal User { get; }

        public GraphQlUserContext(ClaimsPrincipal user) : base()
        {
            User = user;
        }
    }
}
