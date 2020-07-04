using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.GraphQL.Models
{
    public class MovieEvent
    {
        public Guid Id { get; } = Guid.NewGuid();

        public int MovieId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public MovieRating MovieRating { get; set; }
    }
}
