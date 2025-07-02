using Movies.API.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.API.Endpoints.Ratings
{
    public static class RateMovieEndpoint
    {
        public const string Name = "RateMovie";

        public static IEndpointRouteBuilder MapRateMovieEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Rate, async (Guid id, RateMovieRequest rateMovieRequest, IRatingService ratingService, CancellationToken cancellationToken, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await ratingService.RateMovieAsync(id, rateMovieRequest.Rating, userId, cancellationToken);

                return result ? TypedResults.Ok() : Results.NotFound();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            return app;
        }
    }
}
