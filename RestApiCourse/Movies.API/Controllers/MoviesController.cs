using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.API.Auth;
using Movies.API.Endpoints;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.API.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IOutputCacheStore _outputCacheStore;

        public MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore)
        {
            _movieService = movieService;
            _outputCacheStore = outputCacheStore;
        }

        //[ServiceFilter(typeof(ApiKeyAuthFilter))]
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.MapToMovie();

            var result = await _movieService.CreateAsync(movie, cancellationToken);

            await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            var movieResponse = movie.MapToResponse();

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id, }, movie);
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.Get)]
        [OutputCache(PolicyName = "MovieCache")]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, [FromServices] LinkGenerator linkGenerator, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var movie = Guid.TryParse(idOrSlug, out var id) 
                ? await _movieService.GetByIdAsync(id, userId, cancellationToken) 
                : await _movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            if (movie is null)
            {
                return NotFound();
            }

            var movieResponse = movie.MapToResponse();

            movieResponse.Links.Add(new Link
            {
                Rel = "self",
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id })!,
                Type = "GET"
            });

            movieResponse.Links.Add(new Link
            {
                Rel = "self",
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: new { movie.Id })!,
                Type = "PUT"
            });

            movieResponse.Links.Add(new Link
            {
                Rel = "self",
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: new { movie.Id })!,
                Type = "DELETE"
            });

            return Ok(movieResponse);
        }

        [AllowAnonymous]
        [OutputCache(PolicyName = "MovieCache")]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUserId(userId);

            var movies = await _movieService.GetAllAsync(options, cancellationToken);
            var moviesCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

            var moviesResponse = movies.MapToResponse(request.Page, request.PageSize, moviesCount);

            return Ok(moviesResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.MapToMovie(id);
            var userId = HttpContext.GetUserId();

            var updatedMovie = await _movieService.UpdateAsync(movie, userId, cancellationToken);

            if (updatedMovie is null)
            {
                return NotFound();
            }
            await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            var response = movie.MapToResponse();
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            return Ok();
        }
    }
}
