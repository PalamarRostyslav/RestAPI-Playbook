using Microsoft.AspNetCore.OutputCaching;
using Movies.API.Auth;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.API.Endpoints.Movies
{
    public static class UpdateMovieEndpoint
    {
        public const string Name = "UpdateMovie";

        public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut(ApiEndpoints.Movies.Update, async (Guid id, UpdateMovieRequest request, IMovieService movieService, IOutputCacheStore outputCacheStore, HttpContext context, CancellationToken cancellationToken) =>
            {
                var movie = request.MapToMovie(id);
                var userId = context.GetUserId();

                var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);
                if (updatedMovie is null)
                {
                    return Results.NotFound();
                }
                await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                var response = movie.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.AdminUserPolicyName);

            return endpoints;
        }
    }
}
