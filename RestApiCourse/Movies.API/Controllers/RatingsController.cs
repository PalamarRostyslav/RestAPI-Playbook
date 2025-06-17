using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.API.Auth;
using Movies.API.Endpoints;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.API.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
        }

        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest rateMovieRequest, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var result = await _ratingService.RateMovieAsync(id, rateMovieRequest.Rating, userId, cancellationToken);
            
            return result ? Ok() : NotFound();
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
        public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var result = await _ratingService.DeleteRatingAsync(id, userId, cancellationToken);

            return result ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
        public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var ratings = await _ratingService.GetRatingsForUserAsync(userId, cancellationToken);
            var ratingsResponse = ratings.MapToResponse();

            return Ok(ratingsResponse);
        }
    }
}
