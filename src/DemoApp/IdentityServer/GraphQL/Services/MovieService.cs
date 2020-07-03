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

        public MovieService()
        {
            _movies = new List<Movie>();
            for (int i = 1; i <= 5; i++)
            {
                _movies.Add(new Movie { Id = i, Name = Faker.Name.First(), ActorId = i, Company = Faker.Name.First(), MovieRating = Faker.Enum.Random<MovieRating>(), ReleaseDate = DateTimeOffset.Now.AddDays(Faker.RandomNumber.Next(0, 100) * -1) });
            }
        }

        public Task<Movie> GetByIdAsync(int id)
        {
            return Task.FromResult(_movies.SingleOrDefault(x => x.Id == id));
        }

        public Task<IEnumerable<Movie>> GetAsync()
        {
            return Task.FromResult(_movies.AsEnumerable());
        }

        public Task<Movie> Add(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
