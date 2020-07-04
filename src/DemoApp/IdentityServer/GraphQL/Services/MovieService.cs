using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.GraphQL.Models;

namespace IdentityServer.GraphQL.Services
{
    public interface IMovieService
    {
        Task<Movie> GetByIdAsync(int id);
        Task<IEnumerable<Movie>> GetAsync();
        Task<Movie> Add(Movie movie);
    }

    public class MovieService : IMovieService
    {
        private IList<Movie> _movies;
        private readonly IMovieEventService _eventService;

        public MovieService(IMovieEventService eventService)
        {
            _eventService = eventService;
            _movies = new List<Movie>();
            for (int i = 1; i <= 5; i++)
            {
                _movies.Add(new Movie { Id = i, Name = Faker.Name.First(), ActorId = i, Company = Faker.Name.First(), MovieRating = Faker.Enum.Random<MovieRating>(), ReleaseDate = DateTimeOffset.Now.AddDays(Faker.RandomNumber.Next(0, 100) * -1) });
            }
        }

        public Task<Movie> GetByIdAsync(int id)
        {
            var movie = _movies.SingleOrDefault(x => x.Id == id);
            return Task.FromResult(movie ?? throw new ArgumentException($"提供的 {nameof(id)} 无效。"));
        }

        public Task<IEnumerable<Movie>> GetAsync()
        {
            return Task.FromResult(_movies.AsEnumerable());
        }

        public Task<Movie> Add(Movie movie)
        {
            if (movie?.Id == null || _movies.Any(x => x.Id == movie.Id)) throw new ArgumentException($"提供的 {nameof(movie)} 无效。");
            _movies.Add(movie);

            var movieEvent = new MovieEvent
            {
                Name = "Add Movie",
                MovieId = movie.Id,
                TimeStamp = DateTimeOffset.Now,
                MovieRating = movie.MovieRating
            };
            _eventService.AddEvent(movieEvent);

            return Task.FromResult(movie);
        }
    }
}
