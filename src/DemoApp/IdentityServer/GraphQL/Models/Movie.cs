using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.GraphQL.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string Company { get; set; }
        public int ActorId { get; set; }
        public MovieRating MovieRating { get; set; }
    }

    [Flags]
    public enum MovieRating
    {
        Unrated = 0,
        G = 1,
        PG = 2,
        PG13 = 3,
        R = 4,
        NC17 =5
    }
}
