using Movies.API.Auth;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.API.Endpoints.Ratings
{
    public static class GetUserRatingsEndpoint
    {
        public const string Name = "GetUserRatings";

        public static IEndpointRouteBuilder MapGetUserRatingsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async (IRatingService ratingService, CancellationToken cancellationToken, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var ratings = await ratingService.GetRatingsForUserAsync(userId, cancellationToken);
                var ratingsResponse = ratings.MapToResponse();

                return TypedResults.Ok(ratingsResponse);
            })
            .WithName(Name)
            .Produces<IEnumerable<MovieRatingResponse>>(StatusCodes.Status200OK)
            .RequireAuthorization();

            return app;
        }
    }
}
