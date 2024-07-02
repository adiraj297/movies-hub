## Getting Started
1. To run the app:
```shell
dotnet run --project MovieHub.Api/MovieHub.Api.csproj
```
2. To run all the integration tests:
```shell
dotnet test
```
## API Routes:

1. Get all movies 
- Method: GET
- Path: ```/api/movies```

2. Get movie data by movieId
- Method: GET
- Path: ```/api/movies/{movieId}```
- Note: This route is currently not functioning as intended as the external API this flow depends on is being deprecated. However, I have still decided to keep it as part of this project as it showcases how external API calls can be done.  

3. Get reviews for a movie
- Method: GET
- Path: ```/api/movies/{movieId}/reviews```

4. Add a new review for a movie
- Method: POST
- Path: ```/api/movies/{movieId}/reviews```

5. Update an existing review for a movie
- Method: PUT
- Path: ```/api/movies/{movieId}/reviews/{reviewId}```

6. Update a review partially for a movie
- Method: PATCH
- Path: ```/api/movies/{movieId}/reviews/{reviewId}```

7. Delete a review for a movie
- Method: DELETE
- Path: ```/api/movies/{movieId}/reviews/{reviewId}```


## MovieHub DB mapping:
![](MovieHubDB.png)
