using Movies.API.Auth;
using Movies.Application.Services;

namespace Movies.API.Endpoints.Ratings
{
    public static class DeleteRatingEndpoint
    {
        public const string Name = "DeleteRating";

        public static IEndpointRouteBuilder MapDeleteRatingEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (Guid id, IRatingService ratingService, CancellationToken cancellationToken, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await ratingService.DeleteRatingAsync(id, userId, cancellationToken);

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
