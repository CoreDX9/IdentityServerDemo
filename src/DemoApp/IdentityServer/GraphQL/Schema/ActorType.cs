using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using IdentityServer.GraphQL.Models;

namespace IdentityServer.GraphQL.Schema
{
    public class ActorType : ObjectGraphType<Actor>
    {
        public ActorType()
        {
            Name = nameof(Actor);
            Description = "";

            Field(x => x.Id);
            Field(x => x.Name);
        }
    }
}
