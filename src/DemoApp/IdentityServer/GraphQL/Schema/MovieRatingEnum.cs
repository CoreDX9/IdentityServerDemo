using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using IdentityServer.GraphQL.Models;

namespace IdentityServer.GraphQL.Schema
{
    public class MovieRatingEnum : EnumerationGraphType<MovieRating>
    {
        public MovieRatingEnum()
        {
            Name = $"{nameof(MovieRating)}s";
            Description = "";

            foreach (var value in Enum.GetValues(typeof(MovieRating)))
            {
                var name = Enum.GetName(typeof(MovieRating), value);
                AddValue(name, name, value);
            }
        }
    }
}
