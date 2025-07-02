using Movies.API.Auth;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.API.Endpoints.Movies
{
    public static class GetAllMoviesEndpoint
    {
        public const string Name = "GetAllMovies";
        public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(ApiEndpoints.Movies.GetAll, async ([AsParameters] GetAllMoviesRequest getAllMoviesRequest, IMovieService movieService, HttpContext context, CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var options = getAllMoviesRequest.MapToOptions().WithUserId(userId);

                var movies = await movieService.GetAllAsync(options, cancellationToken);
                var moviesCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

                var moviesResponse = movies.MapToResponse(
                    getAllMoviesRequest.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                    getAllMoviesRequest.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                    moviesCount);

                return TypedResults.Ok(moviesResponse);
            })
            .WithName($"{Name}V1")
            .Produces<MoviesResponse>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .CacheOutput("MovieCache")
            .HasApiVersion(1.0);


            endpoints.MapGet(ApiEndpoints.Movies.GetAll, async ([AsParameters] GetAllMoviesRequest getAllMoviesRequest, IMovieService movieService, HttpContext context, CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var options = getAllMoviesRequest.MapToOptions().WithUserId(userId);

                var movies = await movieService.GetAllAsync(options, cancellationToken);
                var moviesCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

                var moviesResponse = movies.MapToResponse(
                    getAllMoviesRequest.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                    getAllMoviesRequest.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                    moviesCount);

                return TypedResults.Ok(moviesResponse);
            })
           .WithName($"{Name}V2")
           .Produces<MoviesResponse>(StatusCodes.Status200OK)
           .WithApiVersionSet(ApiVersioning.VersionSet)
           .CacheOutput("MovieCache")
           .HasApiVersion(2.0);

            return endpoints;
        }
    }
}
