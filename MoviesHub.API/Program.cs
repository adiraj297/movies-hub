using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using MoviesHub.Clients;
using MoviesHub.Database;
using MoviesHub.Repositories;
using MoviesHub.Services;
using Polly;

Console.WriteLine("********************************************");
Console.WriteLine("*                                          *");
Console.WriteLine("*        Welcome to MovieHub API!          *");
Console.WriteLine("*                                          *");
Console.WriteLine("*  This is a .NET app which returns data   *");
Console.WriteLine("*     for movies playing in cinemas        *");
Console.WriteLine("*                                          *");
Console.WriteLine("*            Copyright © 2024              *");
Console.WriteLine("*          All rights reserved.            *");
Console.WriteLine("********************************************");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Console.WriteLine();
Console.WriteLine("Registering endpoint controllers....");
Console.WriteLine();
builder.Services.AddControllers(options => 
        options.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();



// services
Console.WriteLine();
Console.WriteLine("Setting up database context....");
Console.WriteLine();

var dbConnectionString = builder.Configuration["ConnectionStrings:DBConnectionString"];
builder.Services.AddDbContext<MovieHubContext>(dbContextOptions => 
    dbContextOptions.UseSqlite(dbConnectionString));

builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = ctx =>
        ctx.ProblemDetails.Extensions.Add("serverName: ", Environment.MachineName)
);
builder.Services.AddScoped<IMovieHubRepository, MovieHubRepository>();
builder.Services.AddSingleton<IPrincessTheatreClient, PrincessTheatreClient>();
builder.Services.AddHttpClient(PrincessTheatreClient.clientName, // resilience 
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["PrincessTheatreAPI:BaseUrl"]!);
    }).AddResilienceHandler("ResiliencePipeline" , resilienceBuilder =>
{
    resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
    {
        BackoffType = DelayBackoffType.Exponential,
        MaxRetryAttempts = 5,
        UseJitter = true
    });
});

builder.Services.AddTransient<PriceFetchService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

Console.WriteLine();
Console.WriteLine($"Environment is: {app.Environment.EnvironmentName}");
if (app.Environment.EnvironmentName == "Development")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<MovieHubContext>();
        context.Database.EnsureCreated();
        
        var connection = new SqliteConnection(dbConnectionString);
        connection.Open();
        MovieHubDbInitializer.Initialize(context, connection);
        connection.Close();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
}

app.UseRouting();
app.UseEndpoints(endpoint => endpoint.MapControllers());
app.UseHttpsRedirection();

app.Run();

public partial class Program { }
