using GraphQL;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Subscription;
using GraphQL.Types;
using IdentityServer.GraphQL.Models;
using IdentityServer.GraphQL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace IdentityServer.GraphQL.Schema
{
    public class MoviesSubscription : global::GraphQL.Types.ObjectGraphType
    {
        private readonly IMovieEventService _eventService;

        public MoviesSubscription(IMovieEventService eventService)
        {
            _eventService = eventService;

            Name = nameof(MoviesSubscription);

            AddField(new global::GraphQL.Types.EventStreamFieldType
            {
                Name = nameof(MovieEvent),
                Arguments = new global::GraphQL.Types.QueryArguments(new global::GraphQL.Types.QueryArgument<global::GraphQL.Types.ListGraphType<MovieRatingEnum>>
                {
                    Name = $"{nameof(MovieRating)}s"
                }),
                Type = typeof(MovieEventType),
                Resolver = new global::GraphQL.Resolvers.FuncFieldResolver<MovieEvent>(ResolveEvent),
                Subscriber = new global::GraphQL.Resolvers.EventStreamResolver<MovieEvent>(Subscripte)
            });
        }

        private MovieEvent ResolveEvent(IResolveFieldContext arg)
        {
            return arg.Source as MovieEvent;
        }

        private IObservable<MovieEvent> Subscripte(IResolveEventStreamContext arg)
        {
            var ratingList = arg.GetArgument<IList<MovieRating>>($"{nameof(MovieRating)}s", new List<MovieRating>());

            if (ratingList.Any())
            {
                MovieRating ratings = MovieRating.Unrated;

                foreach (var rating in ratingList)
                {
                    ratings = ratings | rating;
                }

                return _eventService.EventStream().Where(e => (e.MovieRating & ratings) == e.MovieRating);
            }
            else return _eventService.EventStream();
        }
    }
}
