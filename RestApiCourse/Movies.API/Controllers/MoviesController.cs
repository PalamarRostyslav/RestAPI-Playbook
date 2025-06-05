using Microsoft.AspNetCore.Mvc;
using Movies.API.Endpoints;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.API.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService; 

        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.MapToMovie();

            var result = await _movieService.CreateAsync(movie, cancellationToken);

            var movieResponse = movie.MapToResponse();

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id, }, movie);
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken cancellationToken)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) 
                ? await _movieService.GetByIdAsync(id, cancellationToken) 
                : await _movieService.GetBySlugAsync(idOrSlug, cancellationToken);

            if (movie is null)
            {
                return NotFound();
            }

            var movieResponse = movie.MapToResponse();

            return Ok(movieResponse);
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var movies = await _movieService.GetAllAsync(cancellationToken);

            var moviesResponse = movies.MapToResponse();

            return Ok(moviesResponse);
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.MapToMovie(id);

            var updatedMovie = await _movieService.UpdateAsync(movie, cancellationToken);

            if (updatedMovie is null)
            {
                return NotFound();
            }

            var response = movie.MapToResponse();
            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
