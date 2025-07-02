using Movies.Application.Services;
using Movies.Contracts.Responses;
using Movies.API.Auth;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Movies.API.Mapping;

namespace Movies.API.Endpoints.Movies
{
    public static class GetMovieEndpoint
    {
        public const string Name = "GetMovie";

        public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(ApiEndpoints.Movies.Get, async (string idOrSlug, IMovieService movieService, HttpContext context, LinkGenerator linkGenerator, CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, cancellationToken)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

                if (movie is null)
                {
                    return Results.NotFound();
                }

                var movieResponse = movie.MapToResponse();

                movieResponse.Links.Add(new Link
                {
                    Rel = "self",
                    // Href = linkGenerator.GetPathByAction(context, nameof(Get), values: new { idOrSlug = movie.Id })!,
                    Href = "test",
                    Type = "GET"
                });

                movieResponse.Links.Add(new Link
                {
                    Rel = "self",
                    Href = linkGenerator.GetPathByAction(context, nameof(Update), values: new { movie.Id })!,
                    Type = "PUT"
                });

                movieResponse.Links.Add(new Link
                {
                    Rel = "self",
                    //linkGenerator.GetPathByAction(context, nameof(Delete), values: new { movie.Id })!,
                    Href = "test",
                    Type = "DELETE"
                });

                return TypedResults.Ok(movieResponse);
            })
            .WithName(Name)
            .CacheOutput("MovieCache")
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            return endpoints;
        }
    }
}
