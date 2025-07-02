namespace Movies.API.Endpoints.Ratings
{
    public static class RatingEndpointExtension
    {
        public static IEndpointRouteBuilder MapRatingEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDeleteRatingEndpoint();
            endpoints.MapGetUserRatingsEndpoint();
            endpoints.MapRateMovieEndpoint();

            return endpoints;
        }
    }
}
