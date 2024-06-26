using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace MoviesHub.Database;

public class MovieHubDbInitializer
{
    public static void Initialize(MovieHubContext context, DbConnection connection)
    {
            if (!context.Movie.Any())
            { 
                Console.WriteLine();
                Console.WriteLine("Seeding the database....");
                Console.WriteLine();
                using SqliteCommand command = new();
                command.CommandText = File.ReadAllText("seed.sql");
                command.Connection = (SqliteConnection)connection;
                command.ExecuteNonQuery();
                Console.WriteLine();
                Console.WriteLine("Database seeding complete!");
                Console.WriteLine();
            }
    }
}