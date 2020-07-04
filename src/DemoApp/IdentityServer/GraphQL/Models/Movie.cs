using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.GraphQL.Models
{
    [Flags]
    public enum MovieRating
    {
        Unrated = 0,
        G = 1,
        PG = 1 << 1,
        PG13 = 1 << 2,
        R = 1 << 3,
        NC17 = 1 << 4
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string Company { get; set; }
        public int ActorId { get; set; }
        public MovieRating MovieRating { get; set; }
    }

    public class MovieInput
    {
        public string Name { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string Company { get; set; }
        public int ActorId { get; set; }
        public MovieRating MovieRating { get; set; }
    }
}
