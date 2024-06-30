using MoviesHub.Startup;

SplashScreen.DisplaySplash();

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.Register(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.SeedData(configuration);
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
} else {
    app.UseExceptionHandler();
}

app.UseRouting();
app.UseEndpoints(endpoint => endpoint.MapControllers());
app.UseHttpsRedirection();

app.Run();

public partial class Program { }
