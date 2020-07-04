using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using IdentityServer.GraphQL.Models;
using IdentityServer.GraphQL.Services;

namespace IdentityServer.GraphQL.Schema
{
    public class MovieType : ObjectGraphType<Movie>
    {
        public MovieType(IActorService actorService)
        {
            Name = nameof(Movie);
            Description = "";

            Field(x => x.Id);
            Field(x => x.Company);
            Field(x => x.Name);
            Field(x => x.ReleaseDate);
            Field(x => x.ActorId);
            Field<MovieRatingEnum>(nameof(MovieRating), resolve: context => context.Source.MovieRating);

            Field<ActorType>(nameof(Actor), resolve: context => actorService.GetByIdAsync(context.Source.ActorId));
        }
    }
}
