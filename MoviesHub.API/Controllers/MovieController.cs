using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Models;
using MoviesHub.Repositories;
using MoviesHub.Services;

namespace MoviesHub.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController(
    IMapper mapper,
    IMovieHubRepository movieHubRepository,
    ILogger<MovieController> logger,
    PriceFetchService priceFetchService)
    : ControllerBase
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IMovieHubRepository _movieHubRepository = movieHubRepository ?? throw new ArgumentNullException(nameof(movieHubRepository));
    private readonly PriceFetchService _priceFetchService = priceFetchService ??  throw new ArgumentNullException(nameof(priceFetchService));
    private readonly ILogger<MovieController> _logger = logger ??  throw new ArgumentNullException(nameof(logger));

    [ApiVersion(1)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAllMovies(
        [FromQuery] string? title, 
        [FromQuery] string? genre)
    {   
        _logger.LogInformation("Now fetching all movies");
        var movieEntities = await _movieHubRepository.GetMoviesAsync(title, genre);
        if (!movieEntities.Any()) return NotFound("No movies playing right now");
        return Ok(_mapper.Map<IEnumerable<MovieDTO>>(movieEntities));
    }
    
    
    [ApiVersion(1, Deprecated = true)]
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieWithMovieCinemaDTO>> GetMovieDetails([FromRoute] int id)
    {
        var movieEntity = await _movieHubRepository.GetMovieWithAdditionalDetailsAsync(id);
        if (movieEntity is null) return NotFound($"Could not find movie details for movie with {id}");     
        
        _logger.LogInformation($"Now fetching details for movie with id: {id}");
        var movieDetailsDto = _mapper.Map<MovieWithMovieCinemaDTO>(movieEntity);
        var movieDetailsResponse =
            await _priceFetchService.AllocatePrincessPricesForMovie(movieEntity.princessTheatreMovieId, movieDetailsDto);
        
        return Ok(movieDetailsResponse);
    }
}