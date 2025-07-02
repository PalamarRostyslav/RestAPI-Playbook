using Microsoft.AspNetCore.OutputCaching;
using Movies.API.Auth;
using Movies.Application.Services;

namespace Movies.API.Endpoints.Movies
{
    public static class DeleteMovieEndpoint
    {
        public const string Name = "DeleteMovie";

           public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder endpoints)
            {
                endpoints.MapDelete(ApiEndpoints.Movies.Delete, async (Guid id, IMovieService movieService, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken) =>
                {
                    var deleted = await movieService.DeleteByIdAsync(id, cancellationToken);

                    if (!deleted)
                    {
                        return Results.NotFound();
                    }

                    await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                    return TypedResults.Ok();
                })
                .WithName(Name)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthConstants.AdminUserPolicyName);
    
                return endpoints;
        }
    }
}
