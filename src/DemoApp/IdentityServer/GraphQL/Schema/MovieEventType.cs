using IdentityServer.GraphQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.GraphQL.Schema
{
    public class MovieEventType : global::GraphQL.Types.ObjectGraphType<MovieEvent>
    {
        public MovieEventType()
        {
            Name = nameof(MovieEvent);

            Field(x => x.Id);
            Field(x => x.MovieId);
            Field(x => x.Name);
            Field(x => x.TimeStamp);
            Field(x => x.MovieRating, type: typeof(MovieRatingEnum));
        }
    }
}
