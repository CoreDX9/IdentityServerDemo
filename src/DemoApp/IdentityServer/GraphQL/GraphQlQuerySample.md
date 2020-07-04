# Query
```
query Query{
  movies{
    id,
    name,
    releaseDate,
    movieRating,
    actor{
      id,
      name
    }
  }
}
```

# Mutation
```
mutation Create($input: MovieInput!){
  createMovie(movie: $input){
    id,
    name,
    releaseDate,
    movieRating,
    actor{
      id,
      name
    }
  }
}
```

## Query variables for mutation
```
{
  "input": {
    "name": "A Movie",
    "releaseDate": "2020-1-1",
    "company": "C1",
    "actorId": 1,
    "movieRating": "R"
  }
}
```

# Subscription
```
subscription Subscription {
  movieEvent(movieRatings:[G, PG, PG13]){
    id,
    movieId,
    name,
    timeStamp,
    movieRating
  }
}
```