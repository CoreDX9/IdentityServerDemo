using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.GraphQL.Models;

namespace IdentityServer.GraphQL.Schema
{
    public class MovieInputType : global::GraphQL.Types.InputObjectGraphType<MovieInput>
    {
        public MovieInputType()
        {
            Name = nameof(MovieInput);

            Field(x => x.Name);
            Field(x => x.ReleaseDate);
            Field(x => x.Company);
            Field(x => x.ActorId);
            Field(x => x.MovieRating, type: typeof(MovieRatingEnum));
        }
    }
}
