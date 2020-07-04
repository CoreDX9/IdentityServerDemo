using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.GraphQL.Services;
using IdentityServer.GraphQL.Models;
using GraphQL;

namespace IdentityServer.GraphQL.Schema
{
    public class MociesMutation : global::GraphQL.Types.ObjectGraphType
    {
        public MociesMutation(IMovieService service)
        {
            Name = nameof(MociesMutation);

            FieldAsync<MovieType>(
                "createMovie"
                , arguments: new global::GraphQL.Types.QueryArguments(new global::GraphQL.Types.QueryArgument<global::GraphQL.Types.NonNullGraphType<MovieInputType>> { Name = "movie" })
                , resolve: async context =>
                 {
                     var movieInput = context.GetArgument<MovieInput>("movie");
                     var movie = new Movie
                     {
                         Id = (await service.GetAsync()).Max(x => x.Id) + 1,
                         Name = movieInput.Name,
                         ActorId = movieInput.ActorId,
                         Company = movieInput.Company,
                         ReleaseDate = movieInput.ReleaseDate,
                         MovieRating = movieInput.MovieRating
                     };

                     return await service.Add(movie);
                 });
        }
    }
}
