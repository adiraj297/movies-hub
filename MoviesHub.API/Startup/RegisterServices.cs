using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using MoviesHub.Clients;
using MoviesHub.Database;
using MoviesHub.Repositories;
using MoviesHub.Services;
using Polly;

namespace MoviesHub.Startup;

public static class RegisterServices
{
    public static void Register(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration["ConnectionStrings:DBConnectionString"];
        
        Console.WriteLine();
        Console.WriteLine("Registering endpoint controllers....");
        Console.WriteLine();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers(options => 
                options.ReturnHttpNotAcceptable = true)
            .AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
        
        Console.WriteLine();
        Console.WriteLine("Setting up database context....");
        Console.WriteLine();
        
        services.AddDbContext<MovieHubContext>(dbContextOptions => 
            dbContextOptions.UseSqlite(dbConnectionString));
        
        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = ctx =>
                ctx.ProblemDetails.Extensions.Add("serverName: ", Environment.MachineName)
        );
        services.AddScoped<IMovieHubRepository, MovieHubRepository>();
        services.AddSingleton<IPrincessTheatreClient, PrincessTheatreClient>();
        services.AddHttpClient(PrincessTheatreClient.clientName, // resilience 
            client =>
            {
                client.BaseAddress = new Uri(configuration["PrincessTheatreAPI:BaseUrl"]!);
            }).AddResilienceHandler("ResiliencePipeline" , resilienceBuilder =>
        {
            resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 5,
                UseJitter = true
            });
        });
        
        services.AddTransient<PriceFetchService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}