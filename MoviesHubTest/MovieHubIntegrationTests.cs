using System.Net;
using System.Net.Http.Json;
using MoviesHub.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace MoviesHubTest;

public class MovieHubIntegrationTests : IClassFixture<TestWebAppFactory<Program>>
    {
        
        [Fact]
        public async Task ShouldGetMovies()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.GetAsync("/api/movies");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var moviesList = JsonSerializer.Serialize(new List<MovieDTO>{
                new () {
                    id = 1,
                    title = "Movie test",
                    director = "Test director",
                    genre = "Action",
                    releaseDate = new DateOnly(1999, 05, 19),
                    runtime = 8650,
                    synopsis = "test synopsis",
                    rating = "PG-13",
                    averageScore = "8.80"
                }});

            Assert.Equal(moviesList, responseString);
        }
        
        [Fact]
        public async Task ShouldGetReviews()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.GetAsync("/api/movies/1/reviews");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var reviewList = JsonSerializer.Serialize(new List<MovieReviewDTO>{
                new () {
                    id = 1,
                    movieId = 1,
                    score = new decimal(8.8),
                    comment = "Amazing movie",
                    reviewDate = DateTime.Parse("2024-05-01 10:30:00").ToString("dd-MM-yyyy HH:mm:ss")
                }});

            Assert.Equal(reviewList, responseString);
        }
        
        [Fact]
        public async Task ShouldNotGetReviewsForNonExistentMovie()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.GetAsync("/api/movies/2/reviews");
           
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldPostReviews()
        {
            var review = new MovieReviewCreationDTO
            {
                score = new decimal(7.1),
                comment = "test"
            };
            var requestData = JsonContent.Create(review);
            
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.PostAsync("/api/movies/1/reviews", requestData);
            response.EnsureSuccessStatusCode();

            var responseDto = await response.Content.ReadFromJsonAsync<MovieReviewDTO>();
            Assert.Equal(responseDto?.score, review.score);
            Assert.Equal(responseDto?.comment, review.comment);
            Assert.NotNull(responseDto?.reviewDate);
        }
        
        [Fact]
        public async Task ShouldNotPostReviewsForNonExistentMovie()
        {
            var review = new MovieReviewCreationDTO
            {
                score = new decimal(7.1),
                comment = "test"
            };
            var requestData = JsonContent.Create(review);
            
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.PostAsync("/api/movies/2/reviews", requestData);
            var responseCode = response.StatusCode;
            Assert.Equal(HttpStatusCode.NotFound, responseCode);
        }
        
        [Fact]
        public async Task ShouldPutReviews()
        {
            var reviewToUpdate = new MovieReviewUpdateDTO
            {
                score = new decimal(7.2),
                comment = "Good movie"
            };
            var requestData = JsonContent.Create(reviewToUpdate);
            var client = new TestWebAppFactory<Program>().CreateClient();
            var putResponse = await client.PutAsync("/api/movies/1/reviews/1", requestData);
            putResponse.EnsureSuccessStatusCode();
            
            var getResponse = await client.GetAsync("/api/movies/1/reviews");
            getResponse.EnsureSuccessStatusCode();
            
            var responseDto = await getResponse.Content.ReadFromJsonAsync<IEnumerable<MovieReviewDTO>>();
            var updatedReview = responseDto?.FirstOrDefault();
            
            Assert.Equal(updatedReview?.score, reviewToUpdate.score);
            Assert.Equal(updatedReview?.comment, reviewToUpdate.comment);
        }
        
        [Fact]
        public async Task ShouldNotPutReviewsForNonExistentMovie()
        {
            var reviewToUpdate = new MovieReviewUpdateDTO()
            {
                score = new decimal(7.2),
                comment = "Good movie"
            };
            var requestData = JsonContent.Create(reviewToUpdate);
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.PutAsync("/api/movies/2/reviews/1", requestData);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ShouldPatchReview() { 
            var patchData = new[]
            {
                new
                {
                    op = "replace",
                    path = "/comment",
                    value = "Good movie"
                }
            };
            
            var requestData = JsonContent.Create(patchData);
            var client = new TestWebAppFactory<Program>().CreateClient();
            var patchResponse = await client.PatchAsync("/api/movies/1/reviews/1", requestData);
            patchResponse.EnsureSuccessStatusCode();
            
            var getResponse = await client.GetAsync("/api/movies/1/reviews");
            getResponse.EnsureSuccessStatusCode();
            
            var responseDto = await getResponse.Content.ReadFromJsonAsync<IEnumerable<MovieReviewDTO>>();
            var updatedReview = responseDto?.FirstOrDefault();
            
            Assert.Equal("Good movie", updatedReview?.comment);
        }
        
        [Fact]
        public async Task ShouldNotPatchReviewWithBadRequest() { 
            var patchData = new[]
            {
                new
                {
                    op = "replace",
                    path = "/releaseDate",
                    value = "Good movie"
                }
            };
            
            var requestData = JsonContent.Create(patchData);
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.PatchAsync("/api/movies/1/reviews/1", requestData);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldNotPatchReviewForNonExistentMovie() { 
            var patchData = new[]
            {
                new
                {
                    op = "replace",
                    path = "/comment",
                    value = "Good movie"
                }
            };
            
            var requestData = JsonContent.Create(patchData);
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.PatchAsync("/api/movies/1/reviews/2", requestData);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldNotPutReviewsForNonExistentReview()
        {
            var reviewToUpdate = new MovieReviewUpdateDTO()
            {
                score = new decimal(7.2),
                comment = "Good movie"
            };
            var requestData = JsonContent.Create(reviewToUpdate);
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.PutAsync("/api/movies/1/reviews/2", requestData);
            
            Assert.Equal(HttpStatusCode.NotFound,  response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldDeleteReviews()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var deleteResponse = await client.DeleteAsync("/api/movies/1/reviews/1");
            deleteResponse.EnsureSuccessStatusCode();
            
            var getResponse = await client.GetAsync("/api/movies/1/reviews");
            getResponse.EnsureSuccessStatusCode();
            var reviewList = await getResponse.Content.ReadFromJsonAsync<IEnumerable<MovieReviewDTO>>();
            Assert.Empty(reviewList!);
        }
        
            
        [Fact]
        public async Task ShouldNotDeleteReviewsForNonExistentMovie()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.DeleteAsync("/api/movies/2/reviews/1");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldNotDeleteReviewsForNonExistentReview()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.DeleteAsync("/api/movies/1/reviews/2");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldGetMovieDetails()
        {
            var factory = new TestWebAppFactory<Program>();
            var client = factory.CreateClient();
        
            var mockedFilmDto = new PrincessTheatreResponseDTO
            {
                provider = "Film world",
                movies = new []
                {
                    new PrincessMovies
                    {
                        id = "fw123",
                        title = "movie",
                        type = "movie",
                        poster = "poster",
                        actors = "Harrison Ford, Mark Hamill, Carrie Fisher, Adam Driver",
                        price = new decimal(25.99)
                    }
                }
            };
            
            var mockedCinemaDto = new PrincessTheatreResponseDTO
            {
                provider = "Cinema world",
                movies = new []
                {
                    new PrincessMovies
                    {
                        id = "cw123",
                        title = "movie",
                        type = "movie",
                        poster = "poster",
                        actors = "Harrison Ford, Mark Hamill, Carrie Fisher, Adam Driver",
                        price = new decimal(19.3)
                    }
                }
            };
            
            factory.StubHttpRequest("cinemaworld/movies", HttpStatusCode.OK, 
                mockedCinemaDto);
            
            factory.StubHttpRequest("filmworld/movies", 
               HttpStatusCode.OK, mockedFilmDto);
           
            
            var response = await client.GetAsync("/api/movies/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var moviesDetails = JsonSerializer.Serialize(new MovieWithMovieCinemaDTO{
                    id = 1,
                    title = "Movie test",
                    director = "Test director",
                    genre = "Action",
                    releaseDate = new DateOnly(1999, 05, 19),
                    runtime = 8650,
                    synopsis = "test synopsis",
                    rating = "PG-13",
                    averageScore = "8.80",
                    movieCinemas = new List<MovieCinemaDTO>
                    {
                        new()
                        {
                            location = "Springfield, QLD",
                            name = "Event cinemas",
                            showTime = new DateOnly(2024, 5, 22),
                            ticketPrice = new decimal(22.5)
                        }
                    },
                    cinemaWorldTicketPrice = new decimal(19.3),
                    filmWorldTicketPrice = new decimal(25.99)
                });
        
            Assert.Equal(moviesDetails, responseString);
        }
        
        [Fact]
        public async Task ShouldNotGetMovieDetailsForNonExistentMovies()
        {
            var client = new TestWebAppFactory<Program>().CreateClient();
            var response = await client.GetAsync("/api/movies/2");
        
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
    }
