using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.GraphQL.Models;

namespace IdentityServer.GraphQL.Services
{
    public interface IActorService
    {
        Task<Actor> GetByIdAsync(int id);
        Task<IEnumerable<Actor>> GetAsync();
    }

    public class ActorService : IActorService
    {
        private IList<Actor> actors;

        public ActorService()
        {
            actors = new List<Actor>();

            for (int i = 1; i <= 5; i++)
            {
                actors.Add(new Actor { Id = i, Name = Faker.Name.FullName() });
            }
        }

        public Task<Actor> GetByIdAsync(int id)
        {
            return Task.FromResult(actors.SingleOrDefault(x => x.Id == id));
        }

        public Task<IEnumerable<Actor>> GetAsync()
        {
            return Task.FromResult(actors.AsEnumerable());
        }
    }
}
