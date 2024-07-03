using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Models;
using MoviesHub.Repositories;

namespace MoviesHub.Controllers;

[ApiController]
[Route("api/movies/{movieId}/reviews")]
public class MovieReviewsController(
    IMapper mapper,
    IMovieHubRepository movieHubRepository,
    ILogger<MovieReviewsController> logger)
    : ControllerBase
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IMovieHubRepository _movieHubRepository = movieHubRepository ?? throw new ArgumentNullException(nameof(movieHubRepository));
    private readonly ILogger<MovieReviewsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [ApiVersion(1)]
    [HttpGet(Name = "GetReviewsForMovie")]
    public async Task<ActionResult<IEnumerable<MovieReviewDTO>>> GetReviewsForMovie(int movieId)
    {
        var movieExists = !await _movieHubRepository.MovieExistsAsync(movieId);
        if (movieExists) return NotFound($"Could not find movie with id: {movieId}");
        
        _logger.LogInformation($"Now fetching all reviews for movie with id: {movieId}");
        var reviewsForMovie = await _movieHubRepository.GetReviewsForMovieAsync(movieId);
        
        return Ok(_mapper.Map<IEnumerable<MovieReviewDTO>>(reviewsForMovie));
    }
    
    [ApiVersion(1)]
    [HttpPost]
    public async Task<ActionResult<MovieReviewDTO>> CreateMovieReview(
        int movieId,
        MovieReviewCreationDTO movieReviewCreationDto
        )
    {
        var movieExists = !await _movieHubRepository.MovieExistsAsync(movieId);
        if (movieExists) return NotFound($"Could not find movie with id: {movieId}");
        
        _logger.LogInformation($"Now creating review for movie with id: {movieId}");
        
        var reviewEntity = _mapper.Map<Entities.MovieReview>(movieReviewCreationDto);
        await _movieHubRepository.CreateReviewForMovieAsync(movieId, reviewEntity);
        await _movieHubRepository.SaveChangesAsync();
        var createdMovieDto = _mapper.Map<MovieReviewDTO>(reviewEntity);

        return CreatedAtRoute("GetReviewsForMovie",
            new
            {
                movieId
            }, createdMovieDto);
    }
    
    [ApiVersion(1)]
    [HttpPut("{reviewId}")]
    public async Task<ActionResult> UpdateReviewForMovie(
        int movieId, 
        int reviewId, 
        MovieReviewUpdateDTO movieReviewUpdateDtoDto)
    {
        var movieExists = !await _movieHubRepository.MovieExistsAsync(movieId);
        if (movieExists) return NotFound($"Could not find movie with id: {movieId}");
        
        _logger.LogInformation($"Now updating review with id: {reviewId} for movie with id: {movieId}");
        
        var reviewForMovieEntity = await _movieHubRepository.GetReviewForMovieAsync(reviewId);
        if (reviewForMovieEntity is null) return NotFound($"Could not find review with id: {reviewId} for this movie with id: {movieId}");
        reviewForMovieEntity.reviewDate = DateTime.Now;
        
        _mapper.Map(movieReviewUpdateDtoDto, reviewForMovieEntity);
        
        await _movieHubRepository.SaveChangesAsync();

        return NoContent();

    }
    
    [ApiVersion(1)]
    [HttpPatch("{reviewId}")]
    public async Task<ActionResult> UpdateReviewPartiallyForMovie(
        int movieId, 
        int reviewId, 
        JsonPatchDocument<MovieReviewUpdateDTO> patchDocument)
    {
        var movieExists = await _movieHubRepository.MovieExistsAsync(movieId);
        if (!movieExists) return NotFound($"Could not find movie with id: {movieId}");
    
        var reviewForMovieEntity = await _movieHubRepository.GetReviewForMovieAsync(reviewId);
        if (reviewForMovieEntity is null) return NotFound($"Could not find review with id: {reviewId} for this movie with id: {movieId}");
        reviewForMovieEntity.reviewDate = DateTime.Now;

        _logger.LogInformation($"Now updating review with id: {reviewId} for movie with id: {movieId}");
        
        var reviewToPatch = _mapper.Map<MovieReviewUpdateDTO>(reviewForMovieEntity);
        patchDocument.ApplyTo(reviewToPatch, ModelState); // apply changes
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!TryValidateModel(reviewToPatch)) return BadRequest(ModelState);
    
        _mapper.Map(reviewToPatch, reviewForMovieEntity);
        
        await _movieHubRepository.SaveChangesAsync();
    
        return NoContent();
    
    }
    
    [ApiVersion(1)]
    [HttpDelete("{reviewId}")]
    public async Task<ActionResult> DeleteReviewForMovie(
        int movieId, 
        int reviewId)
    {
        var movieExists = !await _movieHubRepository.MovieExistsAsync(movieId);
        if (movieExists) return NotFound($"Could not find movie with id: {movieId}");
        
        var reviewForMovie = await _movieHubRepository.GetReviewForMovieAsync(reviewId);
        if (reviewForMovie is null) return NotFound($"Could not find review with id: {reviewId} for this movie with id: {movieId}");
        
        _logger.LogInformation($"Now deleting review with id: {reviewId} for movie with id: {movieId}");
        
        _movieHubRepository.DeleteReviewForMovie(reviewForMovie);
        await _movieHubRepository.SaveChangesAsync();

        return NoContent();

    }
    
    
}