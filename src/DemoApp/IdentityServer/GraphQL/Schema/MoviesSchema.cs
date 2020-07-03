using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.GraphQL.Schema
{
    public class MoviesSchema : global::GraphQL.Types.Schema
    {
        public MoviesSchema(
            IServiceProvider services) :base(services)
        {
            Query = services.GetRequiredService<MoviesQuery>();
        }
    }
}
