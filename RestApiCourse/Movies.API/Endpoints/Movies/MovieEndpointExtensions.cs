namespace Movies.API.Endpoints.Movies
{
    public static class MovieEndpointExtensions
    {
        public static IEndpointRouteBuilder MapMovieEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGetMovie();
            endpoints.MapCreateMovie();
            endpoints.MapGetAllMovies();
            endpoints.MapUpdateMovie();
            endpoints.MapDeleteMovie();

            return endpoints;
        }
    }
}
