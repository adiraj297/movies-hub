using Microsoft.Data.Sqlite;
using MoviesHub.Database;

namespace MoviesHub.Startup;

public static class Seeder
{
    public static void SeedData(this WebApplication app, IConfiguration configuration)
    {
        var dbConnectionString = configuration["ConnectionStrings:DBConnectionString"];
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
}