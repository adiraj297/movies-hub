using MoviesHub.Models;

namespace MoviesHub.Clients;

public class PrincessTheatreClient: IPrincessTheatreClient
{
    
    private readonly string _apiKey;
    private readonly IHttpClientFactory _httpClientFactory;
    public const string clientName = "princessTheatreApi";
    private readonly ILogger<PrincessTheatreClient> _logger;
    
    public PrincessTheatreClient(
        IConfiguration configuration, 
        IHttpClientFactory httpClientFactory, 
        ILogger<PrincessTheatreClient> logger)
    {
        _apiKey = configuration["PrincessTheatreAPI:ApiKey"] ??
                  throw new ArgumentNullException(nameof(configuration));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
    }

    public async Task<PrincessTheatreResponseDTO?> GetMoviesForProvider(string provider)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _logger.LogInformation($"Attempting to get princess theatre movies data for provider: {provider}");
        HttpResponseMessage response = await client.GetAsync($"{provider}/movies");
        return await response.Content.ReadFromJsonAsync<PrincessTheatreResponseDTO>();
    }
}