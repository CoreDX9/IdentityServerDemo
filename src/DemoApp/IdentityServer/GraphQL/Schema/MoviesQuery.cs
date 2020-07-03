using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using IdentityServer.GraphQL.Services;

namespace IdentityServer.GraphQL.Schema
{
    public class MoviesQuery : ObjectGraphType
    {
        public MoviesQuery(IMovieService movieService)
        {
            Name = "Query";

            Field<ListGraphType<MovieType>>("movies", resolve: context => movieService.GetAsync());
        }
    }
}
