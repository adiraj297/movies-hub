using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Moq.Protected;
using MoviesHub.Database;
using MoviesHub.Entities;
using Newtonsoft.Json;
namespace MoviesHubTest;

public class TestWebAppFactory<A> : WebApplicationFactory<Program>
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IHttpClientFactory> _mockFactory;
    
    public TestWebAppFactory()
    {   
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "TEST");
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var client = new HttpClient(_mockHttpMessageHandler.Object);
        client.BaseAddress = new Uri("https://challenge.lexicondigital.com.au/api/v2/"); 
        _mockFactory = new Mock<IHttpClientFactory>();
        _mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        builder.ConfigureTestServices(sp =>
        {
            sp.AddSingleton(m => _mockFactory.Object);
        });
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var configurationValues = new Dictionary<string, string>
            {
                { "PrincessTheatreAPI:ApiKey", "mock-api-key" },
            };
            config.AddInMemoryCollection(configurationValues!);
        });
        
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MovieHubContext>));
            if (dbContext != null) services.Remove(dbContext);

            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
        
            services.AddDbContext<MovieHubContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryMovieHubTest");
                options.UseInternalServiceProvider(serviceProvider);
            });
            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                using (var movieHubDbContext = scope.ServiceProvider.GetRequiredService<MovieHubContext>())
                {
                    try
                    {
                        movieHubDbContext.Database.EnsureCreated();
                        InitializeDbWithData(movieHubDbContext);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error has occured: ", ex);
                    }
                }
            }
        });
    }


    private static void InitializeDbWithData(MovieHubContext db)
    {
        db.Movie.Add(new Movie()
        {
            title = "Movie test",
            director = "Test director",
            genre = "Action",
            releaseDate = new DateOnly(1999, 05, 19),
            runtime = 8650,
            synopsis = "test synopsis",
            rating = "PG-13",
            princessTheatreMovieId = "123"
        });

        db.Cinema.Add(new Cinema()
        {
            location = "Springfield, QLD",
            name = "Event cinemas"
        });
        
        db.MovieCinema.Add(new MovieCinema()
        {
          movieId = 1,
          cinemaId = 1,
          showTime = new DateOnly(2024, 5, 22),
          ticketPrice = new decimal(22.5)
        });

        db.MovieReview.Add(new MovieReview
        {
            movieId = 1,
            score = new decimal(8.8),
            comment = "Amazing movie",
            reviewDate = DateTime.Parse("2024-05-01 10:30:00")
        });
        
        db.SaveChanges();
    }
    
    public void StubHttpRequest<T>(string requestUrl, HttpStatusCode statusCode, T content)
    {
        StubHttpRequest(requestUrl, statusCode, JsonConvert.SerializeObject(content));
    }

    private void StubHttpRequest(string requestUrl, HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg => 
                    msg.RequestUri!.ToString().EndsWith(requestUrl, StringComparison.InvariantCultureIgnoreCase)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content),
            });
    }
}