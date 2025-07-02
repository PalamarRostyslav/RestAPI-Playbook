using Microsoft.AspNetCore.OutputCaching;
using Movies.API.Auth;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.API.Endpoints.Movies
{
    public static class CreateMovieEndpoint
    {
        public const string Name = "CreateMovie";

        public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(ApiEndpoints.Movies.Create, async (CreateMovieRequest request, IMovieService movieService, HttpContext context, IOutputCacheStore cacheStore, CancellationToken cancellationToken) =>
            {
                var movie = request.MapToMovie();

                var result = await movieService.CreateAsync(movie, cancellationToken);

                await cacheStore.EvictByTagAsync("movies", cancellationToken);

                var movieResponse = movie.MapToResponse();

                return TypedResults.CreatedAtRoute(movieResponse, GetMovieEndpoint.Name, new { idOrSlug = movie.Id });
            })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

            return endpoints;
        }
    }
}
